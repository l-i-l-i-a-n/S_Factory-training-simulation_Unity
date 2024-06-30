using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// <summary>
//      Script qui gère le pointeur laser dans la main droite du personnage
// </summary>
public class RaycastTool : MonoBehaviour
{
    private bool _saisieEnCours = false;
    private Rigidbody _objetSaisi_rigidbody = null;

    public CharacterController playerController;
    public LineRenderer rayRenderer;
    public GameObject player;
    public GameObject distributeur;

    [Tooltip("Types d'objets instanciables par le distributeur (placer des préfabs ici)")]
    public List<GameObject> objects;

    [Tooltip("Largeur du pointeur laser")]
    public float rayWidth = 0.05f;

    void Awake()
    {
        rayRenderer.startWidth = rayWidth;
        rayRenderer.endWidth = rayWidth;
    }

    // Late Update fonctionne de manière très similaire à Update, mais se lance après toutes les fonctions "Update"
    // ici cela peut éviter que changer la position du playerController n'entre en conflit avec le script PlayerMovement
    void LateUpdate()
    {
        GameObject _touche;

        // Gère les effets du pointeur laser
        if (Physics.Raycast(transform.position, transform.forward, out var hitInfo))
        {
            rayRenderer.SetPosition(0, transform.position);
            rayRenderer.SetPosition(1, hitInfo.point);
     
            Debug.DrawRay(transform.position, transform.forward, Color.blue);

            // Gestion du clic gauche : saisie / interaction
            if (Input.GetMouseButtonDown(0))
            {
                _touche = hitInfo.transform.gameObject;

                // Vérifie l'action à exécuter avec l'objet touché (saisie ou interaction avec un bouton)
                if (IsSaisissable(_touche))
                {
                    if (!_saisieEnCours) TakeObject(_touche); // Si objet saisissable et non saisi : saisir l'objet
                    else ReleaseObject(); // Si saisissable et saisi : le relâcher
                } 
                else {
                    switch (_touche.tag)
                    {   // Déclenchera l'ouverture de la porte à la prochaine frame dans le Update du GameManager
                        case "BoutonPorte": GameManager.openDoor = true;
                                            break;
                        // Fait sortir un nouvel objet du distributeur
                        case "BoutonDistrib": GenerateRandomObject();
                                              break;
                        // Ferme l'application
                        case "ExitButton": GameManager.ExitGame();
                                           break;
                    }
                }
            }

            // Gestion du clic droit : téléportation
            if (Input.GetMouseButtonDown(1))
            {
                Teleport(hitInfo.point);
            }
        }
        else
        {
            // Si clic gauche quand laser ne pointe pas un objet :
            // relâche l'objet saisi 
            // Par sécurité mais a priori inutile car l'objet saisi est toujours devant le laser
            if (Input.GetMouseButtonDown(0) && _saisieEnCours) ReleaseObject();

            rayRenderer.SetPosition(0, transform.position);
            rayRenderer.SetPosition(1, transform.position + transform.forward*1000f);
        }
    }

    // <summary>
    //      Instancie un nouvel objet au niveau du distributeur
    //
    //      Pioche aléatoirement un type d'objet dans la liste des objets disponibles (peluche ours, pinguin, etc.)
    //      Instancie un objet de ce type
    //      Lui applique une rotation aléatoire (pour le fun)
    // </summary>
    void GenerateRandomObject()
    {
        int i = Random.Range(0, objects.Count);
        Instantiate(objects[i], distributeur.transform.position + new Vector3(-0.4f, 0, 0.7f), Quaternion.Euler(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90)));
    }

    // <summary>
    //      Vérifie qu'un objet fait partie des objets saisissables
    // </summary>
    bool IsSaisissable(GameObject _obj)
    {
        return (_obj.CompareTag("SaisissableBEAR") ||
                _obj.CompareTag("SaisissableBEAR_Present") ||
                _obj.CompareTag("SaisissablePENGUIN") ||
                _obj.CompareTag("SaisissablePENGUIN_Present") ||
                _obj.CompareTag("SaisissableRABBIT") ||
                _obj.CompareTag("SaisissableRABBIT_Present") ||
                _obj.CompareTag("Jetable"));
    }

    // <summary>
    //      Saisit un objet
    //      
    //      Réinitialise la cinématique de l'objet (évite que l'objet dérive s'il était en mouvement au moment de la saisie)
    //      Enlève la gravité sur l'objet
    //      Met l'objet en tant qu'enfant de l'objet Main droite
    //      Place l'objet devant la main
    // </summary>
    void TakeObject(GameObject _obj)
    {
        GameManager.objetSaisi = _obj;
        _objetSaisi_rigidbody = _obj.GetComponent<Rigidbody>();

        _saisieEnCours = true;
        _objetSaisi_rigidbody.velocity = Vector3.zero;
        _objetSaisi_rigidbody.angularVelocity = Vector3.zero;
        _objetSaisi_rigidbody.useGravity = false;
        GameManager.objetSaisi.transform.parent = transform.parent;
        GameManager.objetSaisi.transform.position = transform.parent.transform.position;
        GameManager.objetSaisi.transform.rotation = new Quaternion(0, 0, 0, 0);
        GameManager.objetSaisi.transform.localRotation = new Quaternion(0, 180, 0, 0);
        GameManager.objetSaisi.transform.Translate(Vector3.back + Vector3.down * 0.1f);

        // Met l'objet en mode trigger pour qu'il traverse le décor quand saisi
        // Plus pratique que si on laisse les collisions
        GameManager.objetSaisi.GetComponent<Collider>().isTrigger = true;
    }

    // <summary>
    //      Relâche un objet saisi
    //      
    //      Réactive la gravité sur l'objet
    //      Réactive les collisions
    //      Remet l'objet en tant qu'enfant de la scène
    // </summary>
    void ReleaseObject()
    {
        //Debug.Log("Relâchement");
        GameManager.objetSaisi.GetComponent<Rigidbody>().useGravity = true;
        GameManager.objetSaisi.GetComponent<Collider>().isTrigger = false;
        GameManager.objetSaisi.transform.parent = null;

        _saisieEnCours = false;
        GameManager.objetSaisi = null;
        _objetSaisi_rigidbody = null;
    }

    // <summary>
    //      Téléporte le joueur à la position pointée par le laser au sol
    // </summary>
    void Teleport(Vector3 _point)
    {
        playerController.enabled = false;

        if (_point.y <= 0.25) // Ne téléporte que si le sol est visé ou un peu au dessus
            player.transform.position = _point + Vector3.up; // Vector3.up corrige un problème où le personnage se retrouve
                                                                    // parfois dans le sol à l'arrivée de la téléportation
        playerController.enabled = true;
    }
}