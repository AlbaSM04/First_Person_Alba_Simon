using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class Enemigo : MonoBehaviour
{
    private NavMeshAgent agent; // Controlador de movimiento del enemigo en el NavMesh.
    private FirstPerson player; // Referencia al jugador (clase `FirstPerson`).
    private Animator anim; // Controlador de animaciones del enemigo.
    private Rigidbody[] huesos; // Lista de huesos del enemigo para simular ragdoll (f�sica).

    private bool ventanaAbierta; // Indica si el enemigo est� en una ventana de ataque.

    [SerializeField] private float dano; // Cantidad de da�o que inflige el enemigo.
    [SerializeField] private Transform puntoAtaque; // Punto desde donde se calcula el ataque.
    [SerializeField] private float radioAtaque; // Radio de alcance del ataque.
    [SerializeField] private LayerMask queEsDano; // Define qu� capas pueden recibir da�o.
    private bool danoHecho; // Controla si el da�o ya ha sido infligido en esta ventana de ataque.

    [SerializeField] private float vidas; // Vida actual del enemigo.
    public float Vidas { get => vidas; set => vidas = value; } // Propiedad p�blica para acceder/modificar las vidas.

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Obtiene el componente NavMeshAgent del enemigo.
        player = GameObject.FindObjectOfType<FirstPerson>(); // Encuentra al jugador autom�ticamente.
        anim = GetComponent<Animator>(); // Obtiene el componente Animator.
        huesos = GetComponentsInChildren<Rigidbody>(); // Obtiene todos los huesos del enemigo.

        CambiarEstadoHuesos(true); // Desactiva la f�sica de los huesos al inicio (enemigo r�gido).
    }

    void Update()
    {
        // Establece como destino al jugador.
        agent.SetDestination(player.transform.position);

        // Si el enemigo ha llegado al jugador, detiene el movimiento y ataca.
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true; // Detiene el movimiento del enemigo.
            anim.SetBool("Atacking", true); // Activa la animaci�n de ataque.
            EnfocarPlayer(); // Hace que el enemigo mire hacia el jugador.
        }

        // Si la ventana de ataque est� abierta y no se ha infligido da�o, intenta atacar.
        if (ventanaAbierta && danoHecho == false)
        {
            DetectarJugador(); // Detecta si el jugador est� en el radio de ataque.
        }
    }

    // Hace que el enemigo mire hacia el jugador.
    private void EnfocarPlayer()
    {
        Vector3 direccionAPlayer = (player.transform.position - this.gameObject.transform.position).normalized; // Calcula la direcci�n hacia el jugador.
        direccionAPlayer.y = 0; // Evita rotaci�n en el eje Y (vertical).
        Quaternion.LookRotation(direccionAPlayer); // Gira el enemigo hacia el jugador.
    }

    // Detecta si el jugador est� dentro del radio de ataque.
    private void DetectarJugador()
    {
        Collider[] collsDetectados = Physics.OverlapSphere(puntoAtaque.position, radioAtaque, queEsDano); // Busca colisiones en el radio de ataque.
        if (collsDetectados.Length > 0) // Si hay colisiones detectadas...
        {
            for (int i = 0; i < collsDetectados.Length; i++)
            {
                collsDetectados[i].GetComponent<FirstPerson>().Dano(dano); // Inflige da�o al jugador.
            }
            danoHecho = true; // Marca el da�o como realizado para evitar repetici�n.
        }
    }

    // L�gica para cuando el enemigo muere.
    public void Morir()
    {
        agent.enabled = false; // Desactiva el NavMeshAgent para detener el movimiento.
        anim.enabled = false; // Desactiva el Animator para evitar m�s animaciones.
        CambiarEstadoHuesos(false); // Activa la f�sica de los huesos para simular ragdoll.
        Destroy(gameObject, 10); // Destruye al enemigo despu�s de 10 segundos.
    }

    // Activa o desactiva la f�sica de los huesos.
    private void CambiarEstadoHuesos(bool estado)
    {
        for (int i = 0; i < huesos.Length; i++)
        {
            huesos[i].isKinematic = estado; // Cambia el estado de `isKinematic` (controlado por f�sica o no).
        }
    }

    // Evento que se llama al finalizar la animaci�n de ataque.
    private void FinAtaque()
    {
        agent.isStopped = false; // Reanuda el movimiento del enemigo.
        anim.SetBool("Atacking", false); // Detiene la animaci�n de ataque.
        danoHecho = false; // Resetea el control de da�o.
    }

    // Abre la ventana de ataque (permite infligir da�o).
    private void AbrirVentanaAtaque()
    {
        ventanaAbierta = true;
    }

    // Cierra la ventana de ataque (detiene la posibilidad de infligir da�o).
    private void CerrarVentanaAtaque()
    {
        ventanaAbierta = false;
    }
}