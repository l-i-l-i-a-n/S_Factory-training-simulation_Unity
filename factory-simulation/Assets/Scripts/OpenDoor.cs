using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
//      Script associé à la porte
// </summary>
public class OpenDoor : MonoBehaviour
{
    private AudioSource[] audioSource;

    GameObject doorLeft, doorRight; // Objets enfants porte gauche et droite

    Coroutine currentCoroutine = null; // Conserve la coroutine en cours d'exécution

    [Tooltip("Temps d'ouverture (/fermeture) de la porte (en secondes)")]
    public float animationLength = 0.8f;

    [Tooltip("Temps durant lequel la porte reste ouverte pour laisser passer le joueur (en secondes)")]
    public float waitingLength = 1;

    void Start()
    {
        // Récupération des portes
        doorLeft = transform.GetChild(0).gameObject;
        doorRight = transform.GetChild(1).gameObject;

        audioSource = GetComponents<AudioSource>();
    }

    void FixedUpdate()
    {
        // Si bouton appuyé ET coroutine pas déjà en cours d'exécution
        if (GameManager.openDoor && currentCoroutine == null)
            currentCoroutine = StartCoroutine(TranslateX()); // Déclenchement de la coroutine
    }

    // <summary>
    //      Coroutine d'ouverture + fermeture des portes
    //
    //      Effectue une translation de chaque côté de la porte vers l'extérieur
    //      Laisse passer le joueur durant waitingLength secondes
    //      Puis referme les portes
    //      
    //      Joue un son en même temps
    // </summary>
    IEnumerator TranslateX()
    {
        Vector3 xyz_left, xyz_right;
        float t;

        if (currentCoroutine != null)
            Debug.Log("Already running");

        // Son d'ouverture
        audioSource[0].Play();

        // Calcul des positions initiales de la porte
        xyz_left = doorLeft.transform.position;
        xyz_right = doorRight.transform.position;
        for (t = 0f; t < 1; t += 0.01f) // Ouverture des portes
        {
            doorLeft.transform.position = xyz_left + Vector3.left * (t + 0.01f);
            doorRight.transform.position = xyz_right + Vector3.right * (t + 0.01f);
            yield return new WaitForSeconds(animationLength * 0.01f); // Point où l'exécution se met en pause jusqu'à la prochaine frame
        }

        // Attente pour laisser passer le joueur
        yield return new WaitForSeconds(waitingLength);

        // Son de fermeture
        audioSource[1].Play();

        // Calcul des nouvelles positions portes ouvertes
        xyz_left = doorLeft.transform.position;
        xyz_right = doorRight.transform.position;
        for (t = 0f; t < 1; t += 0.01f) // Fermeture des portes
        {
            doorLeft.transform.position = xyz_left - Vector3.left * (t + 0.01f);
            doorRight.transform.position = xyz_right - Vector3.right * (t + 0.01f);
            yield return new WaitForSeconds(animationLength * 0.01f); // Point où l'exécution se met en pause jusqu'à la prochaine frame
        }

        GameManager.openDoor = false;
        currentCoroutine = null;
    }
}
