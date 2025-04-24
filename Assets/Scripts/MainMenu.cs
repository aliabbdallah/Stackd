using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
public void SummerScene(){
    SceneManager.LoadScene(4);
}
public void WinterScene(){
    SceneManager.LoadScene(5);
}
public void AutumnScene(){
    SceneManager.LoadScene(6);
}
public void MainScene(){
    SceneManager.LoadScene(3);
}
public void LoginScene(){
    SceneManager.LoadScene(1);
}
}

