const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');
const bcrypt = require('bcrypt');
const db = require('./database');
const app = express();
const PORT = 3000;

app.use(cors());
app.use(bodyParser.json());

// Check email availability
app.post('/check-email', (req, res) => {
    const { email } = req.body;

    if (!email) {
        return res.status(400).json({
            success: false,
            error: {
                code: 'MISSING_FIELDS',
                message: 'Email is required'
            }
        });
    }

    const sql = `SELECT COUNT(*) as count FROM users WHERE email = ?`;
    db.get(sql, [email], (err, row) => {
        if (err) {
            return res.status(500).json({ 
                success: false, 
                error: { 
                    code: 'DB_ERROR', 
                    message: err.message 
                } 
            });
        }
        
        // Return true if email is available (count is 0), false if already in use
        res.json(row.count === 0);
    });
});

// Register endpoint
app.post('/register', async (req, res) => {
    const { username, email, password } = req.body;

    // Validate required fields
    if (!username || !email || !password) {
        return res.status(400).json({
            success: false,
            error: {
                code: 'MISSING_FIELDS',
                message: 'All fields are required',
                details: {
                    username: !!username,
                    email: !!email,
                    password: !!password
                }
            }
        });
    }

    try {
        // Hash the password
        const hashedPassword = await bcrypt.hash(password, 10);

        // Insert the new user
        const sql = `INSERT INTO users (username, email, password) VALUES (?, ?, ?)`;
        db.run(sql, [username, email, hashedPassword], function(err) {
            if (err) {
                // Check for unique constraint violation
                if (err.message.includes('UNIQUE constraint failed')) {
                    return res.status(400).json({
                        success: false,
                        error: {
                            code: 'DUPLICATE_EMAIL',
                            message: 'Email already registered',
                            details: { email: true }
                        }
                    });
                }
                return res.status(500).json({
                    success: false,
                    error: {
                        code: 'DB_ERROR',
                        message: 'Failed to register user'
                    }
                });
            }

            // Return success response
            res.json({
                success: true,
                data: {
                    id: this.lastID,
                    username,
                    email
                }
            });
        });
    } catch (error) {
        res.status(500).json({
            success: false,
            error: {
                code: 'SERVER_ERROR',
                message: 'Failed to process registration'
            }
        });
    }
});

// Login
app.post('/login', (req, res) => {
    const { email, password } = req.body;

    if (!email || !password) {
        return res.status(400).json({
            success: false,
            error: {
                code: 'MISSING_FIELDS',
                message: 'Email and password are required',
                details: {
                    email: !!email,
                    password: !!password
                }
            }
        });
    }

    const sql = `SELECT * FROM users WHERE email = ?`;
    db.get(sql, [email], async (err, user) => {
        if (err || !user) {
            return res.status(401).json({ success: false, error: { code: 'USER_NOT_FOUND', message: 'Email not registered' } });
        }

        const match = await bcrypt.compare(password, user.password);
        if (!match) {
            return res.status(401).json({ success: false, error: { code: 'INVALID_PASSWORD', message: 'Incorrect password' } });
        }

        res.json({ success: true, data: { id: user.id, username: user.username, email: user.email } });
    });
});

app.listen(PORT, () => {
    console.log(`Server running on http://localhost:${PORT}`);
}); 