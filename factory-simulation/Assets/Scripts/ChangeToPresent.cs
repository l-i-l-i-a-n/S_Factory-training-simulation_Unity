using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
//      Script associé à la machine d'emballage (sur le Trigger)
// </summary>
public class ChangeToPresent : MonoBehaviour
{
    private GameObject _in, _out;

    [Tooltip("Apparence de paquet cadeau voulue")]
    public GameObject present;

    // <summary>
    //      Transforme une peluche (non jetable) en paquet cadeau
    //
    //      Détruit la peluche et instancie un cadeau avec le bon tag à la même position
    // </summary>
    private void OnTriggerExit(Collider col)
    {
        _in = col.gameObject;
        if (IsEmballable(_in)) 
        {
            _out = Instantiate(present, _in.transform.position, Quaternion.identity);
            _out.tag = _in.tag + "_Present";
            Destroy(_in);
        }
    }

    private bool IsEmballable(GameObject _in)
    {
        return (!_in.CompareTag("Untagged") && // Eviter que le joueur passe dans la machine à emballer
                                               // via un glitch que nous avons identifié (grâce à la téléportation)
                !_in.CompareTag("Jetable") && // Evite d'emballer les objets jetables
                !_in.tag.Contains("_Present") && // Evite d'emballer deux fois le même objet
                _in != GameManager.objetSaisi); // Evite d'emballer par erreur un objet saisi
                                                // (évite aussi la triche en passant directement
                                                // l'objet dans la machine sans passer par le tapis roulant)
    }
}
