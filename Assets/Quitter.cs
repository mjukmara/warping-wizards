﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quitter : MonoBehaviour {

  void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      FadeScreen.instance.QuitApp();
    }
  }

  public void QuitApp() {
    FadeScreen.instance.QuitApp();
  }
}