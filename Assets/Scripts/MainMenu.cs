using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
public void SummerScene(){
    SceneManager.LoadScene(1);
}
public void WinterScene(){
    SceneManager.LoadScene(2);
}
public void AutumnScene(){
    SceneManager.LoadScene(3);
}
public void MainScene(){
    SceneManager.LoadScene(0);
}
}
