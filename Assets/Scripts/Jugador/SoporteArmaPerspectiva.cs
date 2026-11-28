using UnityEngine;

public class SoporteArmaPerspectiva : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerLookController vista;
    [SerializeField] private Transform modeloArma;

    [Header("Puntos de Anclaje")]
    [Tooltip("Objeto vacío hijo de la Main Camera (para 1ª persona).")]
    [SerializeField] private Transform puntoPrimeraPersona;
    [Tooltip("Objeto vacío hijo de la mano derecha o cuerpo del personaje (para 3ª persona).")]
    [SerializeField] private Transform puntoTerceraPersona;

    private bool ultimaPerspectivaTercera;

    private void Awake()
    {
        if (vista == null)
            vista = GetComponent<PlayerLookController>();
    }

    private void Start()
    {
        if (vista != null)
        {
            ultimaPerspectivaTercera = vista.IsThirdPerson;
            AplicarAnclaje(ultimaPerspectivaTercera);
        }
    }

    private void LateUpdate()
    {
        if (vista == null || modeloArma == null) return;

        // Detecta en tiempo real si cambiaste entre 1ª y 3ª persona con T
        if (vista.IsThirdPerson != ultimaPerspectivaTercera)
        {
            ultimaPerspectivaTercera = vista.IsThirdPerson;
            AplicarAnclaje(ultimaPerspectivaTercera);
        }
    }

    private void AplicarAnclaje(bool enTerceraPersona)
    {
        Transform destino = enTerceraPersona ? puntoTerceraPersona : puntoPrimeraPersona;
        if (destino == null) return;

        modeloArma.SetParent(destino, false);
        modeloArma.localPosition = Vector3.zero;
        modeloArma.localRotation = Quaternion.identity;
    }
}