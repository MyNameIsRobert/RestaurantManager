using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{

    public GameObject player;
    NavMeshAgent aiController;
    public float moveSpeed = 1f, scrollSpeed = 1, rotateSpeed = 45;
    public GameObject cube;
    // Start is called before the first frame update
    void Start()
    {
        aiController = player.GetComponent<NavMeshAgent>();
    }
    IEnumerator RotateCamera()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Vector3 vector = Vector3.zero;
        RaycastHit hit;
        Ray ray = new Ray(GetComponent<Camera>().transform.position, GetComponent<Camera>().transform.forward);
        if(Physics.Raycast(ray, out hit))
        {
            vector = hit.point;
        }
        
        while (!Input.GetButtonUp("Alt Use"))
        {
            
            transform.RotateAround(vector, new Vector3(0, Input.GetAxis("Mouse X"), 0), rotateSpeed * Time.deltaTime);
            yield return null;
        }
        Cursor.lockState = CursorLockMode.None;
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Use"))
        {
            Vector3 vector = GetMousePositionOnPlane();
            if (vector != Vector3.zero)
            {
                aiController.SetDestination(vector);
                cube.transform.position = vector;
            }
        }
        transform.Translate(moveSpeed * Time.deltaTime * Input.GetAxis("Horizontal"), moveSpeed * Time.deltaTime * Input.GetAxis("Vertical"), 0);
        if(Input.GetAxis("Scrollwheel") != 0)
        {
            Camera cam = GetComponent<Camera>();
 //           if(cam.orthographicSize >= 1 && cam.orthographicSize <= 10)
                cam.orthographicSize += Input.GetAxis("Scrollwheel") * scrollSpeed;
        }
        if(Input.GetButtonDown("Alt Use"))
        {
            StartCoroutine(RotateCamera());
        }
        
    }

    public Vector3 GetMousePositionOnPlane()
    {
        Vector3 hitpoint = Vector3.zero;
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            
            hitpoint = hit.point;
            if(hit.collider.GetComponent<NavMeshObstacle>())
            {
                hitpoint = Vector3.zero;
            }
        }
        else
        {

            Debug.Log("Hit nothing");
        }

        return hitpoint;
    }

}
