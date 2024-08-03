using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface ISerializableComponent
{
    //Something should be done with these 2 methods to allow making new components easier
    public INetworkSerializable SerializeComponent();
    public void DeserializeComponent(INetworkSerializable serializedComponent);
}
