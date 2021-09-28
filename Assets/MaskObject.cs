using UnityEngine;

//[ExecuteAlways]
public class MaskObject : MonoBehaviour
{
    void Start()
    {
        if(GetComponent<MeshRenderer>())
            GetComponent<MeshRenderer>().material.renderQueue = 3002;   
        else if(GetComponent<SkinnedMeshRenderer>())
            GetComponent<SkinnedMeshRenderer>().material.renderQueue = 3002;
        else if(GetComponent<ParticleSystemRenderer>())
            GetComponent<ParticleSystemRenderer>().material.renderQueue = 3002;
    }

}
