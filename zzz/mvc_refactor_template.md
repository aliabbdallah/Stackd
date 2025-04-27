# Unity MVC Refactor Template

This template guides you through refactoring Unity scripts to follow the Model-View-Controller (MVC) pattern, using examples from your project (e.g., APIManager, BlockMovement, GameManager, etc.).

---

## 1. **Recommended Folder Structure**

```
Assets/
  Scripts/
    Model/
      BlockModel.cs
      GameModel.cs
      UserModel.cs
      ...
    View/
      BlockView.cs
      GameView.cs
      CameraView.cs
      ...
    Controller/
      BlockController.cs
      GameController.cs
      CameraController.cs
      ...
    Services/
      APIManager.cs
      ...
```

---

## 2. **MVC Roles Explained**

- **Model:** Pure data/state. No Unity/MonoBehaviour code. Notifies observers (views/controllers) of changes via events.
- **View:** Handles UI, rendering, and user input. MonoBehaviour scripts. Raises events for user actions.
- **Controller:** Handles business/game logic. Listens to view events, updates models, and instructs views to update.
- **Service:** (e.g., APIManager) Provides backend/data access. Used by controllers via interfaces.

---

## 3. **Refactor Example: Block (BlockMovement.cs)**

### **BlockModel.cs**
```csharp
public class BlockModel
{
    public Vector3 Position { get; set; }
    public bool IsSettled { get; set; }
    // Add events for state changes
    public event Action OnSettled;
    public void Settle()
    {
        IsSettled = true;
        OnSettled?.Invoke();
    }
}
```

### **BlockView.cs**
```csharp
using UnityEngine;
public class BlockView : MonoBehaviour
{
    public void UpdatePosition(Vector3 pos) => transform.position = pos;
    public void PlaySettleAnimation() { /* ... */ }
    // Raise events for user input
    public event Action OnDropRequested;
    void Update() {
        if (Input.GetMouseButtonDown(0)) OnDropRequested?.Invoke();
    }
}
```

### **BlockController.cs**
```csharp
using UnityEngine;
public class BlockController : MonoBehaviour
{
    public BlockModel model;
    public BlockView view;
    void Awake()
    {
        view.OnDropRequested += HandleDrop;
        model.OnSettled += view.PlaySettleAnimation;
    }
    void HandleDrop() { /* update model, start physics, etc. */ }
}
```

---

## 4. **APIManager as a Service**
- Remove direct references to views.
- Expose only methods for controllers to call (e.g., RegisterUser, LoginUser).
- Use interfaces for dependency injection:

```csharp
public interface IAPIService
{
    void RegisterUser(...);
    void LoginUser(...);
    // ...
}

public class APIManager : MonoBehaviour, IAPIService { ... }
```

---

## 5. **GameManager Refactor**
- **GameModel:** Holds game state (score, block list, etc.).
- **GameView:** Handles UI and visual updates.
- **GameController:** Handles game logic, updates model, and tells views to update.
- Use events to notify views of model changes.

---

## 6. **General Best Practices**
- **Models:** No MonoBehaviour, no UnityEngine references.
- **Views:** MonoBehaviour, only handle display/input, raise events for user actions.
- **Controllers:** MonoBehaviour, subscribe to view/model events, update models, call view methods.
- **Services:** Use interfaces, inject into controllers.
- **Events:** Use C# events/delegates for communication.
- **No direct references:** Views never call logic/services directly.

---

## 7. **Session and Camera Example**
- **SessionModel:** Holds session state.
- **SessionController:** Handles validation, updates model, triggers view changes.
- **SessionView:** Handles UI/scene transitions.
- **CameraView/Controller:** CameraView animates, CameraController decides when/how to move.

---

## 8. **Summary Table**
| Script                | Model         | View           | Controller      | Service         |
|-----------------------|--------------|----------------|-----------------|-----------------|
| BlockMovement         | BlockModel    | BlockView      | BlockController |                 |
| GameManager           | GameModel     | GameView       | GameController  |                 |
| CameraFollows         | CameraModel?  | CameraView     | CameraController|                 |
| APIManager            |              |                |                 | APIManager      |
| SessionManager        | SessionModel  | SessionView    | SessionController|                |
| GroundController      | GroundModel?  | GroundView     | GroundController|                 |
| SeasonPhysicsManager  | SeasonPhysics |                |                 |                 |

---

**Use this template as a guide for refactoring each script. Start with one feature (e.g., blocks), then apply the pattern to others.** 