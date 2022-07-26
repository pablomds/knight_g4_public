using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraControl : MonoBehaviour
{
    private Vector3 previousPosition;

    float rotationSpeed = 1;
    public Transform Target, Player;
    float mouseX, mouseY;

    public Transform Obstruction;
    float zoomSpeed = 2f;

    private List<Transform> ObsctructionsToCast;

    private Transform initTransform;

    void Start()
    {
        initTransform = transform;
        ObsctructionsToCast = new List<Transform>();
        Obstruction = Target;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        CamControl();
        ViewObstructed();
    }


    void CamControl()
    {
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -35, 60);

        //Rotate around
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.RotateAround(Target.transform.position, Vector3.up, Input.GetAxis("Mouse X") * 10f);
        }
        else
        {
            Target.rotation = Quaternion.Euler(mouseY, mouseX, 0);
            Player.rotation = Quaternion.Euler(0, mouseX, 0);
        }
    }

    void ViewObstructed()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Target.position - transform.position, out hit, 4.5f))
        {
            if (hit.collider.gameObject.tag != "Player")
            {
                Obstruction = hit.transform;
                if (Obstruction.gameObject.TryGetComponent(out SkinnedMeshRenderer skinMeshRenderer))
                    skinMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                else if (Obstruction.gameObject.TryGetComponent(out MeshRenderer meshRenderer))
                    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

                //Obstruction.gameObject.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

                if (!ObsctructionsToCast.Contains(Obstruction))
                    ObsctructionsToCast.Add(Obstruction);
                /*if (Vector3.Distance(Obstruction.position, transform.position) >= 3f && Vector3.Distance(transform.position, Target.position) >= 1.5f)
                    transform.Translate(Vector3.forward * zoomSpeed * Time.deltaTime);*/
            }
            else
            {
                foreach (Transform oldObstruction in ObsctructionsToCast)
                {
                    //oldObstruction.gameObject.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    if (oldObstruction.gameObject.TryGetComponent(out SkinnedMeshRenderer skinMeshRenderer))
                        skinMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    else if (oldObstruction.gameObject.TryGetComponent(out MeshRenderer meshRenderer))
                        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }

                if (Vector3.Distance(transform.position, Target.position) < -transform.position.z)
                    transform.Translate(Vector3.back * zoomSpeed * Time.deltaTime);
            }
        }
    }
}