//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Data/Input/ToolsMapping/ToolsInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @ToolsInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @ToolsInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""ToolsInput"",
    ""maps"": [
        {
            ""name"": ""Interface"",
            ""id"": ""799a5df1-3719-4b2f-8362-482ac4579d8d"",
            ""actions"": [
                {
                    ""name"": ""OpenClosePanel"",
                    ""type"": ""Value"",
                    ""id"": ""402cc11c-77af-425e-a76e-b0bc696c1d39"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""OpenConsole"",
                    ""type"": ""Value"",
                    ""id"": ""69485d7a-1205-468c-8074-0897201d5318"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e93db6c8-f335-4330-bb4c-e697095be106"",
                    ""path"": ""<Keyboard>/f1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""InterfaceControls"",
                    ""action"": ""OpenClosePanel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d00aa754-54f1-403b-bb74-e5c61541e2a3"",
                    ""path"": ""<Keyboard>/f2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenConsole"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""InterfaceControls"",
            ""bindingGroup"": ""InterfaceControls"",
            ""devices"": []
        }
    ]
}");
        // Interface
        m_Interface = asset.FindActionMap("Interface", throwIfNotFound: true);
        m_Interface_OpenClosePanel = m_Interface.FindAction("OpenClosePanel", throwIfNotFound: true);
        m_Interface_OpenConsole = m_Interface.FindAction("OpenConsole", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Interface
    private readonly InputActionMap m_Interface;
    private List<IInterfaceActions> m_InterfaceActionsCallbackInterfaces = new List<IInterfaceActions>();
    private readonly InputAction m_Interface_OpenClosePanel;
    private readonly InputAction m_Interface_OpenConsole;
    public struct InterfaceActions
    {
        private @ToolsInput m_Wrapper;
        public InterfaceActions(@ToolsInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @OpenClosePanel => m_Wrapper.m_Interface_OpenClosePanel;
        public InputAction @OpenConsole => m_Wrapper.m_Interface_OpenConsole;
        public InputActionMap Get() { return m_Wrapper.m_Interface; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(InterfaceActions set) { return set.Get(); }
        public void AddCallbacks(IInterfaceActions instance)
        {
            if (instance == null || m_Wrapper.m_InterfaceActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_InterfaceActionsCallbackInterfaces.Add(instance);
            @OpenClosePanel.started += instance.OnOpenClosePanel;
            @OpenClosePanel.performed += instance.OnOpenClosePanel;
            @OpenClosePanel.canceled += instance.OnOpenClosePanel;
            @OpenConsole.started += instance.OnOpenConsole;
            @OpenConsole.performed += instance.OnOpenConsole;
            @OpenConsole.canceled += instance.OnOpenConsole;
        }

        private void UnregisterCallbacks(IInterfaceActions instance)
        {
            @OpenClosePanel.started -= instance.OnOpenClosePanel;
            @OpenClosePanel.performed -= instance.OnOpenClosePanel;
            @OpenClosePanel.canceled -= instance.OnOpenClosePanel;
            @OpenConsole.started -= instance.OnOpenConsole;
            @OpenConsole.performed -= instance.OnOpenConsole;
            @OpenConsole.canceled -= instance.OnOpenConsole;
        }

        public void RemoveCallbacks(IInterfaceActions instance)
        {
            if (m_Wrapper.m_InterfaceActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IInterfaceActions instance)
        {
            foreach (var item in m_Wrapper.m_InterfaceActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_InterfaceActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public InterfaceActions @Interface => new InterfaceActions(this);
    private int m_InterfaceControlsSchemeIndex = -1;
    public InputControlScheme InterfaceControlsScheme
    {
        get
        {
            if (m_InterfaceControlsSchemeIndex == -1) m_InterfaceControlsSchemeIndex = asset.FindControlSchemeIndex("InterfaceControls");
            return asset.controlSchemes[m_InterfaceControlsSchemeIndex];
        }
    }
    public interface IInterfaceActions
    {
        void OnOpenClosePanel(InputAction.CallbackContext context);
        void OnOpenConsole(InputAction.CallbackContext context);
    }
}
