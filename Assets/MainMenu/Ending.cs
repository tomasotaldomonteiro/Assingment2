using System;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Ending");
        }
    }
}
