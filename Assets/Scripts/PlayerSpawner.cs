using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ninja;
    [SerializeField] private GameObject ghost;
    private void Awake()
    {
        PlayerInput.Instantiate(ninja, controlScheme: "Keyboard", pairWithDevices: new[] {Keyboard.current});
        PlayerInput.Instantiate(ghost, controlScheme: "Keyboard", pairWithDevices: new[] {Keyboard.current});


        InputSystem.onDeviceChange += DeviceChange;
    }

    private void DeviceChange(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
    {
        print(inputDevice);
        print(inputDeviceChange);
    }
}
