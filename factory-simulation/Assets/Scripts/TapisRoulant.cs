using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
//      Script associ� au tapis roulant
//      
//      Anime la texture de l'objet et
//      fait avancer les objets pos�s dessus
//
//      Le rigidbody doit �tre isKinematic pour que cela fonctionne
// </summary>
public class TapisRoulant : MonoBehaviour
{
    private float _speed;
    private Renderer rendererBegin, rendererMain, rendererEnd;
    private Rigidbody rigidbodyTapis;

    [Tooltip("Parties du tapis roulant")]
    public GameObject begin, main, end;

    void Start()
    {
        rigidbodyTapis = GetComponent<Rigidbody>();
        rendererBegin = begin.GetComponent<Renderer>();
        rendererMain = main.GetComponent<Renderer>();
        rendererEnd = end.GetComponent<Renderer>();

        _speed = GameManager.conveyor_speed;
    }

    void FixedUpdate()
    {
        Vector3 position = rigidbodyTapis.position;

        // Changer la position du rigidbody ne fait pas intervenir le moteur physique
        // => n'impacte pas la position des objets pos�s dessus
        // Fait reculer le rigidbody mais pas les objets dessus
        rigidbodyTapis.position += Vector3.back * _speed * Time.fixedDeltaTime;

        // MovePosition d�place aussi le rigidbody,
        // mais applique une force aux objets pos�s dessus,
        // qui suivent donc le d�placement
        // Fait avancer le rigidbody avec les objets dessus
        rigidbodyTapis.MovePosition(position);

        // Combiner ces deux transformations entraine le d�placement des objets pos�s dessus,
        // tout en laissant le tapis roulant lui-m�me immobile

        // Animation de la texture du tapis roulant
        rendererBegin.material.SetTextureOffset("_MainTex", new Vector2(-Time.time * _speed * 2, 0));
        rendererMain.material.SetTextureOffset("_MainTex", new Vector2(0, Time.time * _speed));
        rendererEnd.material.SetTextureOffset("_MainTex", new Vector2(-Time.time * _speed * 2, 0));
    }
}
