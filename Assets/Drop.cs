using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

public class Drop : MonoBehaviour
{
    void Start()
    {
        // Obtener los nombres de los puertos disponibles en la computadora
        string[] portNames = SerialPort.GetPortNames();

        // Obtener referencia al componente Dropdown en el objeto actual
        Dropdown dropdown = GetComponent<Dropdown>();

        // Llamar a la función para poblar el Dropdown con los nombres de los puertos
        PopulateDropdown(dropdown, portNames);
    }

    void PopulateDropdown(Dropdown dropdown, string[] optionsArray)
    {
        List<string> options = new List<string>(optionsArray);
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }
}