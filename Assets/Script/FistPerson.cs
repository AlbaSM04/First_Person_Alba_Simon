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
    private Camera cam; // Referencia a la cámara principal

    [Header("Gravedad")]
    [SerializeField] private float gravedad; // Escala de la de gravedad aplicada al personaje
    private Vector3 movimientoVertical; // Control del movimiento vertical (gravedad y saltos)
    [SerializeField] private float alturaMaxSalto; // Altura máxima que puede alcanzar al saltar

    [Header("Terreno")]
    [SerializeField] private Transform pies; // Transform que representa la posición de los pies del personaje
    [SerializeField] private float radioDeteccion; // Radio de detección del suelo
    [SerializeField] private LayerMask queEsTerreno; // Máscara para definir qué objetos son considerados terreno
       
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

        // Obtiene la cámara principal de la escena
        cam = Camera.main;

             

        // Configura el cursor del ratón para que esté bloqueado y no sea visible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;        
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(h, v).normalized; // Normaliza el vector de entrada

        // Solo mueve si el jugador está presionando alguna tecla
        if (input.sqrMagnitude > 0)
        {
            // Rota el cuerpo del personaje según la orientación de la cámara
            float anguloRotacion = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            transform.eulerAngles = new Vector3(0, anguloRotacion, 0); // Aplica la rotación del personaje

            // Calcula el movimiento hacia adelante en base al ángulo
            Vector3 direccionMovimiento = Quaternion.Euler(0, anguloRotacion, 0) * Vector3.forward; // Aplica la rotación al movimiento

            // Mueve al personaje hacia la dirección calculada
            controller.Move(direccionMovimiento * velocidad * Time.deltaTime);
        }

        // Aplica la gravedad y detecta si está en el suelo
        AplicarGravedad();
        DeteccionSuelo();

        
    }

    private void AplicarGravedad()
    {
        // Incrementa la velocidad vertical con la gravedad
        if (!EstaEnSuelo())  // Si no está en el suelo, aplica la gravedad
        {
            movimientoVertical.y += gravedad * Time.deltaTime;
        }

        // Aplica el movimiento vertical al CharacterController
        controller.Move(movimientoVertical * Time.deltaTime);
    }

    private void Saltar()
    {
        // Si se presiona la tecla de espacio y está en el suelo
        if (Input.GetKeyDown(KeyCode.Space) && EstaEnSuelo())
        {
            // Calcula la velocidad vertical necesaria para alcanzar la altura del salto
            movimientoVertical.y = Mathf.Sqrt(-2 * gravedad * alturaMaxSalto);
        }
    }

    private bool EstaEnSuelo()
    {
        // Verifica si el jugador está tocando el suelo (usando un pequeño radio)
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
        // Reduce las vidas del personaje según el daño recibido
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
