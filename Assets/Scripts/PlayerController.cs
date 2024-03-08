using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using System.Collections;
using FishNet.Object;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{

    [SerializeField] float Speed;

    Renderer _Renderer;

    [SyncVar(OnChange = nameof(_NewColor))]
    Color _MyColor;

    void Awake()
    {
        _Renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (base.IsOwner == false)
            return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            Color newColor = Random.ColorHSV();
            CambiarColorServidorRPC(newColor);
        }

        Vector3 inputDirection = Vector3.zero;
        inputDirection.x = Input.GetAxis("Horizontal");
        inputDirection.y = Input.GetAxis("Vertical");

        transform.Translate(inputDirection * Speed * Time.deltaTime);
    }

    void _NewColor(Color _OldColor, Color _NewColor, bool _asServer)
    {
        _Renderer.material.color = _NewColor;
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        if (base.Owner.IsLocalClient)
        {
            _Renderer.material.color = Color.green;
            name += "- local";
        }
    }

    [ServerRpc]
    void CambiarColorServidorRPC(Color _Color)
    {
        _MyColor = _Color;
    }

    [ObserversRpc(RunLocally = true)]
    void CambiarColorRPC(Color _color)
    {
        _Renderer.material.color = _color;
    }
}