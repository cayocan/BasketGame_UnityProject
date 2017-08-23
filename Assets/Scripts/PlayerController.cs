using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rigid;
    public GameObject Cabeca;//Pivot da cabeça.
    public GameObject Perna;//Pivot da perna.
    public GameObject Braco;//Pivot dos braços com a main camera.

    private float VelocidadeRotacaoCorpo; //Horizontal.
    private float VelocidadeRotacaoVertical;//Vertical.
    private bool clampVerticalRotation = true;

    private bool PodePular;

    private Quaternion CabecaRot;
    private Quaternion BracoRot;

    [SerializeField]
    [Range(100, 1000)]
    private float ForcaPulo = 300;
    [Range(1, 10)]
    public float VelocidadeAndarFrente = 7;
    [Range(1, 10)]
    public float VelocidadeAndarLado = 5;

    [Range(0, 90)]
    private float MaximumX = 40;
    [Range(-90, 0)]
    private float MinimumX = -40;

    [Range(1, 5)]
    public float SensibilidadeMouseX = 2;
    [Range(1, 5)]
    public float SensibilidadeMouseY = 2;

    // Use this for initialization
    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        rigid.useGravity = true;
        rigid.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        CabecaRot = Cabeca.transform.localRotation;
        BracoRot = Braco.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        this.RotacaoCorpo();
        this.RotacaoVertical();
        this.Movimentacao();
        this.pular();
    }

    private void Movimentacao()//Movimentação genérica do personagem.
    {
        Vector3 posicaoAtual = this.transform.position;
        Vector3 deslocamento;

        if (Input.GetKey("w"))//Frente
        {
            deslocamento = transform.forward * VelocidadeAndarFrente * Time.deltaTime;
            this.transform.position = posicaoAtual + deslocamento;
            Perna.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if (Input.GetKey("s"))//Traz
        {
            deslocamento = -transform.forward * VelocidadeAndarFrente * Time.deltaTime;
            this.transform.position = posicaoAtual + deslocamento;
            Perna.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if (Input.GetKey("d"))//Direita
        {
            transform.Translate(Vector2.right * VelocidadeAndarLado * Time.deltaTime);
            Perna.transform.localRotation = Quaternion.Euler(0, 90, 0);
        }
        if (Input.GetKey("a"))//Esquerda
        {
            transform.Translate(-Vector2.right * VelocidadeAndarLado * Time.deltaTime);
            Perna.transform.localRotation = Quaternion.Euler(0, -90, 0);
        }
        if (Input.GetKey("w") && Input.GetKey("d"))//Diagonal Direita Frente
        {
            Perna.transform.localRotation = Quaternion.Euler(0, 45, 0);
        }
        else if (Input.GetKey("s") && Input.GetKey("d"))//Diagonal Direita Costas
        {
            Perna.transform.localRotation = Quaternion.Euler(0, -45, 0);
        }
        else if (Input.GetKey("w") && Input.GetKey("a"))//Diagonal Esquerda Frente
        {
            Perna.transform.localRotation = Quaternion.Euler(0, -45, 0);
        }
        else if (Input.GetKey("s") && Input.GetKey("a"))//Diagonal Esquerda Costas
        {
            Perna.transform.localRotation = Quaternion.Euler(0, 45, 0);
        }
    }

    private void pular()//Pulo genérico do personagem.
    {
        if (PodePular)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rigid.AddForce(0, ForcaPulo, 0);
                PodePular = false;
            }
        }

        if (PodePular)
        {
            rigid.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            rigid.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | ~RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }
    }

    private void RotacaoCorpo()//Rotação (Horizontal) genérica do corpo do personagem com o mouse. 
    {
        //Caso velocidadeRotacao... == 0 || null, as multiplicações a seguir dariam valor nulo.
        VelocidadeRotacaoCorpo = 50;

        //Retorna a posição do eixo X do mouse.
        VelocidadeRotacaoCorpo = Input.GetAxis("Mouse X") * (VelocidadeRotacaoCorpo * Time.deltaTime) * SensibilidadeMouseX;
        this.transform.Rotate(0, VelocidadeRotacaoCorpo, 0);


    }

    private void RotacaoVertical()//Rotação (Vertical) genérica do corpo do personagem com o mouse. 
    {
        VelocidadeRotacaoVertical = 50;

        VelocidadeRotacaoVertical = Input.GetAxis("Mouse Y") * (VelocidadeRotacaoVertical * Time.deltaTime) * SensibilidadeMouseY;
        //Cabeca.transform.Rotate(-VelocidadeRotacaoCabeca, 0, 0);
        CabecaRot *= Quaternion.Euler(-VelocidadeRotacaoVertical, 0f, 0f);
        BracoRot *= Quaternion.Euler(-VelocidadeRotacaoVertical, 0f, 0f);


        if (clampVerticalRotation)
        {
            CabecaRot = LimiteDeRotacaoVertical(CabecaRot);
            BracoRot = LimiteDeRotacaoVertical(BracoRot);
        }
        Cabeca.transform.localRotation = CabecaRot;
        Braco.transform.localRotation = BracoRot;
    }

    private Quaternion LimiteDeRotacaoVertical(Quaternion q)//Função que define o limite de rotação vertical da cabeça e dos braços.
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    private void OnCollisionEnter(Collision elementoColidido)//Função que detecta colisões em geral.
    {
        if (elementoColidido.gameObject.tag == "Chao")//Verifica se o personagem esta colidindo com o chão.
        {
            PodePular = true;
        }
    }
}
