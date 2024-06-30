using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ReceiveObject : MonoBehaviour
{
    private GameObject _receivedObject;

    [Tooltip("Placer ici le Text qui doit afficher le nombre d'objets sur ce réceptacle")]
    public Text text;

    [Tooltip("Type de peluche attendu sur ce réceptacle (doit correspondre à un tag de peluche existant)")]
    public string type;

    private int localScore = 0;

    // <summary>
    //      Réceptionne un objet sur un réceptacle
    //
    //      Traite les 3 réceptacles principaux et la poubelle (détruit les objets jetables)
    //
    //      Vérifie si l'objet posé correspond au type attendu par CE réceptacle
    //      Si OK, réinitialise les caractéristiques de l'objet précédemment saisi
    //      Et incrémente le score pour ce type d'objet
    //      Sinon, décrémente le score, détruit l'objet, et incrémente le nb. d'erreurs, puis déclenche l'alarme
    // </summary>
    private void OnTriggerEnter(Collider col)
    {
        GameObject obj = col.gameObject;

        // Evite de jeter un objet par erreur en passant sur la poubelle quand l'objet est saisi
        if (obj == GameManager.objetSaisi) return;

        if (type != "Jetable")
        {
            if (obj.CompareTag(type + "_Present"))
            {
                _receivedObject = obj;
                _receivedObject.GetComponent<Rigidbody>().useGravity = true;
                col.isTrigger = false;
                _receivedObject.transform.parent = null;
                _receivedObject.tag = "Untagged";

                localScore++;
                text.text = localScore.ToString();
                GameManager.score++;
            }
            else Mistake(obj);
        }
        else // Si type attendu = Jetable (quand le script est associé à la poubelle)
        {
            if (obj.CompareTag("Jetable"))
            {
                Destroy(obj);
                localScore++;
                text.text = localScore.ToString();
                GameManager.score++;
            }
            else Mistake(obj);
        }
    }

    void Mistake(GameObject _in)
    {
        Destroy(_in);
        localScore--;
        text.text = localScore.ToString();
        GameManager.score--;
        GameManager.nbMistakes++;

        // Déclenche l'alarme à la prochaine frame
        GameManager.triggerAlarm = true;
    }
}
