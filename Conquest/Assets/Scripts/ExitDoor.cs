using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] float secoondsToLoad = 2f;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponent<Animator>().SetTrigger("open");
    }

    public void StartLoadingNextLevel()
    {
        GetComponent<Animator>().SetTrigger("Close");

        StartCoroutine(LoadNextLevel());
    }


    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(secoondsToLoad);

        var currentScemceIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScemceIndex + 1);

    }

}
