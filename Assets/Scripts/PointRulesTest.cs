using UnityEngine;
using System.Collections;

public class PointRulesTest : MonoBehaviour
{
    public GameObject Cestas;
    private int ContadorCestas;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider colisor)
    {
        if (colisor.CompareTag("Bola"))
        {
            ContadorCestas += 1;
            Cestas.GetComponent<TextMesh>().text = "" + ContadorCestas;
        }
    }
}
