using UnityEngine;
using System.Collections;

public class BallInteract : MonoBehaviour
{
    [SerializeField]
    private Camera CameraPrimeiraPessoa;//Pega a câmera primeira pessoa do player.
    [SerializeField]
    private GameObject PivotBola;//Guarda o GameObject do pivot da bola.
    [SerializeField]
    private GameObject ColisorCesta1;

    private Ray RaioDeInteracao;//Guarda o raio que vai sair da minha camera.
    [SerializeField]
    private float AlcanceDeInteracao = 1.8f;//Distancia no qual é possível interagir com um GameObject.
    private RaycastHit hit;//Guarda os valores do GameObject atingido pelo raio.

    private bool EstaComBola;//Guarda se o personagem esta com a bola (true) ou não (false).
    private bool PodePegarBola;
    private float TempoDecorridoLançamento;//Guarda o tempo decorrido após o último lançamento.

    GameObject Bola;

    [SerializeField]
    [Range(1, 200)]
    private float EscalaDeForca = 12;
    private float ForcaInicial;

    [SerializeField]
    [Range(0, 60)]
    private float AnguloDoArremesso;

    // Use this for initialization
    void Start()
    {
        Bola = GameObject.FindGameObjectWithTag("Bola");
        AnguloDoArremesso = -AnguloDoArremesso;
        ForcaInicial = EscalaDeForca;

    }

    // Update is called once per frame
    void Update()
    {
        TempoDecorridoLançamento += Time.deltaTime;

        DefinirAnguloDoArremesso();
        PegarBola();
        VerificaBola();
        Arremesso();
        Passe();

        Debug.Log("Distancia = " + CalcularDistanciaDaCesta());
    }

    private void VerificaBola()//Esta função verifica estados da bola.
    {
        if (Bola.transform.parent != null)//Verifica se tem alguém com a bola.
        {
            EstaComBola = true;
        }
        else
        {
            EstaComBola = false;
        }

        if (TempoDecorridoLançamento >= 1)//Verifica se o personagem arremessou ou tocou a bola a 1 segundo atrás.
        {
            PodePegarBola = true;
        }
        else
        {
            PodePegarBola = false;
        }

    }

    private float CalcularDistanciaDaCesta()
    {
        return Vector3.Distance(this.transform.position, ColisorCesta1.transform.position);
    }

    private void PegarBola()//Função responsável por pegar a bola do chão e colocar na mão do player.
    {
        Vector3 Pivot = PivotBola.transform.position;

        RaioDeInteracao = CameraPrimeiraPessoa.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(RaioDeInteracao, out hit, AlcanceDeInteracao))//Aqui verifico se o objeto que eu miro esta dentro do alcance de interação do player.
        {

            if (hit.collider.tag == "Bola" && EstaComBola == false && PodePegarBola)//Aqui verifico se onde eu estou mirando é uma bola e se essa bola esta fora da mão de um player.
            {
                hit.collider.transform.position = Pivot;
                hit.collider.GetComponent<Rigidbody>().isKinematic = true;//Aqui eu ligo o kinematic para a bola não ser afetada pela física, colisões, etc.
                hit.transform.parent = PivotBola.transform;//Aqui eu faço a bola ser filha do GameObject vazio PivotBola que sinaliza onde a bola fica no player.
                //hit.transform.localRotation = Quaternion.Euler(0,0,0);
            }
        }
    }

    private void Arremesso()
    {
        if (EstaComBola)
        {
            if (Input.GetMouseButton(0))
            {
                EscalaDeForca = EscalaDeForca + Time.deltaTime * 2;

            }
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Força = " + EscalaDeForca);
                TempoDecorridoLançamento = 0;


                PivotBola.transform.localRotation = Quaternion.Euler(AnguloDoArremesso, 0, 0);
                Bola.transform.parent = null;
                Bola.GetComponent<Rigidbody>().isKinematic = false;
                Bola.GetComponent<Rigidbody>().velocity = PivotBola.transform.forward * EscalaDeForca;
                EscalaDeForca = ForcaInicial;
            }           
        }
    }

    private void DefinirAnguloDoArremesso()
    {
        //TODO Implements./
    }

    private void Passe()
    {
        if (EstaComBola)
        {
            if (Input.GetMouseButton(1))
            {
                EscalaDeForca = EscalaDeForca + Time.deltaTime * 100;


            }
            if (Input.GetMouseButtonUp(1))
            {
                TempoDecorridoLançamento = 0;

                PivotBola.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Bola.transform.parent = null;
                Bola.GetComponent<Rigidbody>().isKinematic = false;
                Bola.GetComponent<Rigidbody>().velocity = PivotBola.transform.forward * EscalaDeForca;
                EscalaDeForca = ForcaInicial;
            }
        }
    }
}
