using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.SceneManagement; 

public class FirstPerson : MonoBehaviour
{
    [Header ("Movimiento pesonaje")]
    [SerializeField] private float vidasPersonaje; // Numero de vidas que tiene el personaje
    [SerializeField] TMP_Text textoVidas; 
    [SerializeField] TMP_Text textoEnemigosMuertos; 
    [SerializeField] private float velocidad; // Velocidad de movimiento del personaje
    private Camera cam; // Referencia a la c�mara principal

    [Header("Gravedad")]
    [SerializeField] private float gravedad; // Escala de la de gravedad aplicada al personaje
    private Vector3 movimientoVertical; // Control del movimiento vertical (gravedad y saltos)
    [SerializeField] private float alturaMaxSalto; // Altura m�xima que puede alcanzar al saltar

    [Header("Terreno")]
    [SerializeField] private Transform pies; // Transform que representa la posici�n de los pies del personaje
    [SerializeField] private float radioDeteccion; // Radio de detecci�n del suelo
    [SerializeField] private LayerMask queEsTerreno; // M�scara para definir qu� objetos son considerados terreno
       
    // Con esto podemos referenciar al componente que tenemos del CharacterController que utilizamos para mover al personaje
    private CharacterController controller;

    [Header ("Armas")] 
    [SerializeField] MenuPrincipalArma datosArma; // ScriptableObject con datos del arma principal

    [Header("Canvas")]
    [SerializeField] Canvas muerte;
    [SerializeField] Canvas ganador;
    [SerializeField] Canvas hud;




    void Start()
    {
        textoVidas.SetText("Vidas: " + vidasPersonaje);
        
        // Obtiene el componente CharacterController del objeto
        controller = GetComponent<CharacterController>();

        // Obtiene la c�mara principal de la escena
        cam = Camera.main;

             

        // Configura el cursor del rat�n para que est� bloqueado y no sea visible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;        
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(h, v).normalized; // Normaliza el vector de entrada

        // Solo mueve si el jugador est� presionando alguna tecla
        if (input.sqrMagnitude > 0)
        {
            // Rota el cuerpo del personaje seg�n la orientaci�n de la c�mara
            float anguloRotacion = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            transform.eulerAngles = new Vector3(0, anguloRotacion, 0); // Aplica la rotaci�n del personaje

            // Calcula el movimiento hacia adelante en base al �ngulo
            Vector3 direccionMovimiento = Quaternion.Euler(0, anguloRotacion, 0) * Vector3.forward; // Aplica la rotaci�n al movimiento

            // Mueve al personaje hacia la direcci�n calculada
            controller.Move(direccionMovimiento * velocidad * Time.deltaTime);
        }

        // Aplica la gravedad y detecta si est� en el suelo
        AplicarGravedad();
        DeteccionSuelo();

        
    }

    private void AplicarGravedad()
    {
        // Incrementa la velocidad vertical con la gravedad
        if (!EstaEnSuelo())  // Si no est� en el suelo, aplica la gravedad
        {
            movimientoVertical.y += gravedad * Time.deltaTime;
        }

        // Aplica el movimiento vertical al CharacterController
        controller.Move(movimientoVertical * Time.deltaTime);
    }

    private void Saltar()
    {
        // Si se presiona la tecla de espacio y est� en el suelo
        if (Input.GetKeyDown(KeyCode.Space) && EstaEnSuelo())
        {
            // Calcula la velocidad vertical necesaria para alcanzar la altura del salto
            movimientoVertical.y = Mathf.Sqrt(-2 * gravedad * alturaMaxSalto);
        }
    }

    private bool EstaEnSuelo()
    {
        // Verifica si el jugador est� tocando el suelo (usando un peque�o radio)
        return Physics.CheckSphere(pies.position, 0.3f, queEsTerreno);
    }

    private void DeteccionSuelo()
    {
        // Detecta si hay colisiones dentro del radio de los pies con la capa de suelo
        Collider[] collsDetetados = Physics.OverlapSphere(pies.position, radioDeteccion, queEsTerreno);

        // Si hay colisiones, resetea el movimiento vertical y permite saltar
        if (collsDetetados.Length > 0)
        {
            movimientoVertical.y = 0;
            Saltar();
        }
    }


    public void Dano(float Dano)
    {
        // Reduce las vidas del personaje seg�n el da�o recibido
        vidasPersonaje -= Dano;
        textoVidas.SetText("Vidas: " + vidasPersonaje);

        // Si las vidas llegan a 0 o menos, destruye al personaje y carga la escena de Game Over
        if (vidasPersonaje <= 0)
        {
            hud.gameObject.SetActive(false);
            muerte.gameObject.SetActive(true);
            Destroy(gameObject);
          
        }
    }

   

    
}
