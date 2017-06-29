using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Cube : MonoBehaviour
{
    public enum Direction // 7elements
    {
        non, x_plus, x_minus, y_plus, y_minus, z_plus, z_minus
    }
    public enum Select // 7 elements
    {
        top, bottom, front, back, left, right, center
    }
    public enum State
    {
        rotate, cube, setup, clear
    }

    private GameObject cubes;
    private GameObject[] cube;

    private Vector3 center_parent = new Vector3( 0, 0, 0);
    private Vector3 top_parent = new Vector3( 0, 1, 0);
    private Vector3[] top_children = new Vector3[]
    {
        new Vector3(-1, 1,-1), new Vector3(-1, 1, 0), new Vector3(-1, 1, 1),
        new Vector3( 0, 1,-1),                        new Vector3( 0, 1, 1),
        new Vector3( 1, 1,-1), new Vector3( 1, 1, 0), new Vector3( 1, 1, 1)
    };
    private Vector3[] center_ychildren = new Vector3[8]
    {
        new Vector3(-1, 0,-1), new Vector3(-1, 0, 0), new Vector3(-1, 0, 1),
        new Vector3( 0, 0,-1),                        new Vector3( 0, 0, 1),
        new Vector3( 1, 0,-1), new Vector3( 1, 0, 0), new Vector3( 1, 0, 1)
    };
    private Vector3 bottom_parent = new Vector3( 0, -1, 0);
    private Vector3[] bottom_children = new Vector3[8]
    {
        new Vector3(-1,-1,-1), new Vector3(-1,-1, 0), new Vector3(-1,-1, 1),
        new Vector3( 0,-1,-1),                        new Vector3( 0,-1, 1),
        new Vector3( 1,-1,-1), new Vector3( 1,-1, 0), new Vector3( 1,-1, 1)
    };
    private Vector3 front_parent = new Vector3( 0, 0, 1);
    private Vector3[] front_children = new Vector3[8]
    {
        new Vector3(-1,-1, 1), new Vector3( 0,-1, 1), new Vector3( 1,-1, 1),
        new Vector3(-1, 0, 1),                        new Vector3( 1, 0, 1),
        new Vector3(-1, 1, 1), new Vector3( 0, 1, 1), new Vector3( 1, 1, 1)
    };
    private Vector3[] center_zchildren = new Vector3[8]
        {
                new Vector3(-1,-1, 0), new Vector3( 0,-1, 0), new Vector3( 1,-1, 0),
                new Vector3(-1, 0, 0),                        new Vector3( 1, 0, 0),
                new Vector3(-1, 1, 0), new Vector3( 0, 1, 0), new Vector3( 1, 1, 0)
    };
    private Vector3 back_parent = new Vector3( 0, 0, -1);
    private Vector3[] back_children = new Vector3[8]
    {
        new Vector3(-1,-1,-1), new Vector3( 0,-1,-1), new Vector3( 1,-1,-1),
        new Vector3(-1, 0,-1),                        new Vector3( 1, 0,-1),
        new Vector3(-1, 1,-1), new Vector3( 0, 1,-1), new Vector3( 1, 1,-1)
    };
    private Vector3 left_parent = new Vector3( 1, 0, 0);
    private Vector3[] left_children = new Vector3[8]
    {
        new Vector3( 1,-1,-1), new Vector3( 1,-1, 0), new Vector3( 1,-1, 1),
        new Vector3( 1, 0,-1),                        new Vector3( 1, 0, 1),
        new Vector3( 1, 1,-1), new Vector3( 1, 1, 0), new Vector3( 1, 1, 1)
    };
    private Vector3[] center_xchildren = new Vector3[8]
    {
        new Vector3( 0,-1,-1), new Vector3( 0,-1, 0), new Vector3( 0,-1, 1),
        new Vector3( 0, 0,-1),                        new Vector3( 0, 0, 1),
        new Vector3( 0, 1,-1), new Vector3( 0, 1, 0), new Vector3( 0, 1, 1)
    };
    private Vector3 right_parent = new Vector3(-1, 0, 0);
    private Vector3[] right_children = new Vector3[8]
    {
        new Vector3(-1,-1,-1), new Vector3(-1,-1, 0), new Vector3(-1,-1, 1),
        new Vector3(-1, 0,-1),                        new Vector3(-1, 0, 1),
        new Vector3(-1, 1,-1), new Vector3(-1, 1, 0), new Vector3(-1, 1, 1)
    };

    public Direction direction;
    public Select select;
    public State state;

    private const int frame_rate = 60;
    private const int rotate_frame = 12; // 60frame / 5 = 12frame = 0.2s;
    private const float frame_par_rotate_angle = 7.5f; // 90 / 12 = 15
    private const int shuffle_count = 100;

    void Awake()
    {
        Application.targetFrameRate = frame_rate;
        cubes = GameObject.Find("cubes");
        cube = GameObject.FindGameObjectsWithTag("cube");
        direction = Direction.non;
        select = Select.right;
        state = State.setup;
}

    void Start()
    {
        Shuffle();
    }

    void Update()
    {
        InputKey();
        if (state == State.rotate) Rotate90();
        else if (state == State.cube) CubeRotate90();
    }
    void OnDestroy()
    {

	}

    void OnClick(GameObject obj)
    {
        Debug.Log(obj.name);
    }

private void InputKey()
    {
        if (Input.GetKey(KeyCode.Escape)) Application.Quit();
        direction = Direction.non;
        
        if (state == State.rotate || state == State.cube)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                if (state == State.rotate) state = State.cube;
                else if (state == State.cube) state = State.rotate;
                return;
            }
            else if (Input.GetKeyDown(KeyCode.J)) direction = Direction.x_plus;
            else if (Input.GetKeyDown(KeyCode.K)) direction = Direction.x_minus;
            else if (Input.GetKeyDown(KeyCode.H)) direction = Direction.y_plus;
            else if (Input.GetKeyDown(KeyCode.L)) direction = Direction.y_minus;
            else if (Input.GetKeyDown(KeyCode.O)) direction = Direction.z_plus;
            else if (Input.GetKeyDown(KeyCode.I)) direction = Direction.z_minus;

            else if (Input.GetKeyDown(KeyCode.S)) select = Select.center;
            else if (Input.GetKeyDown(KeyCode.W)) select = Select.top;
            else if (Input.GetKeyDown(KeyCode.X)) select = Select.bottom;
            else if (Input.GetKeyDown(KeyCode.Z)) select = Select.front;
            else if (Input.GetKeyDown(KeyCode.E)) select = Select.back;
            else if (Input.GetKeyDown(KeyCode.A)) select = Select.left;
            else if (Input.GetKeyDown(KeyCode.D)) select = Select.right;
        }
    }

    private IEnumerator Rotate()
    {
        if (direction == Direction.non) yield break;
        if (state != State.rotate && state != State.setup) yield break;


        if (direction == Direction.y_plus || direction == Direction.y_minus)
        {
            if (select == Select.top)
            {
                GameObject parent = GetCubeByPosition(top_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.y_plus) rotate_angle = new Vector3(0, 1, 0);
                else if (direction == Direction.y_minus) rotate_angle = new Vector3(0, -1, 0);
                else yield break;

                rotate_angle *= frame_par_rotate_angle;
                Group(top_parent, top_children);
                for (int i = 0; i < rotate_frame; ++i)
                {
                    parent.transform.Rotate(rotate_angle, Space.World);
                    yield return null;
                }
                Ungroup(top_parent);

            }
            else if (select == Select.center)
            {
                GameObject parent = GetCubeByPosition(center_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.y_plus) rotate_angle = new Vector3(0, 1, 0);
                else if (direction == Direction.y_minus) rotate_angle = new Vector3(0, -1, 0);
                else yield break;

                rotate_angle *= frame_par_rotate_angle;
                Group(center_parent, center_ychildren);
                for (int i = 0; i < rotate_frame; ++i)
                {
                    parent.transform.Rotate(rotate_angle, Space.World);
                    yield return null;
                }
                Ungroup(center_parent);

            }
            else if (select == Select.bottom)
            {
                GameObject parent = GetCubeByPosition(bottom_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.y_plus) rotate_angle = new Vector3(0, 1, 0);
                else if (direction == Direction.y_minus) rotate_angle = new Vector3(0, -1, 0);
                else yield break;

                rotate_angle *= frame_par_rotate_angle;
                Group(bottom_parent, bottom_children);
                for (int i = 0; i < rotate_frame; ++i)
                {
                    parent.transform.Rotate(rotate_angle, Space.World);
                    yield return null;
                }
                Ungroup(bottom_parent);
            }
        }
        else if (direction == Direction.z_plus || direction == Direction.z_minus)
        {
            if (select == Select.front)
            {
                GameObject parent = GetCubeByPosition(front_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.z_plus) rotate_angle = new Vector3(0, 0, 1);
                else if (direction == Direction.z_minus) rotate_angle = new Vector3(0, 0, -1);
                else yield break;

                rotate_angle *= frame_par_rotate_angle;
                Group(front_parent, front_children);
                for (int i = 0; i < rotate_frame; ++i)
                {
                    parent.transform.Rotate(rotate_angle, Space.World);
                    yield return null;
                }
                Ungroup(front_parent);
            }
            else if (select == Select.center)
            {
                GameObject parent = GetCubeByPosition(center_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.z_plus) rotate_angle = new Vector3(0, 0, 1);
                else if (direction == Direction.z_minus) rotate_angle = new Vector3(0, 0, -1);
                else yield break;

                rotate_angle *= frame_par_rotate_angle;
                Group(center_parent, center_zchildren);
                for (int i = 0; i < rotate_frame; ++i)
                {
                    parent.transform.Rotate(rotate_angle, Space.World);
                    yield return null;
                }
                Ungroup(center_parent);
            }
            else if (select == Select.back)
            {
                GameObject parent = GetCubeByPosition(back_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.z_plus) rotate_angle = new Vector3(0, 0, 1);
                else if (direction == Direction.z_minus) rotate_angle = new Vector3(0, 0, -1);
                else yield break;

                rotate_angle *= frame_par_rotate_angle;
                Group(back_parent, back_children);
                for (int i = 0; i < rotate_frame; ++i)
                {
                    parent.transform.Rotate(rotate_angle, Space.World);
                    yield return null;
                }
                Ungroup(back_parent);
            }
        }
        else if (direction == Direction.x_plus || direction == Direction.x_minus)
        {
            if (select == Select.left)
            {
                GameObject parent = GetCubeByPosition(left_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.x_plus) rotate_angle = new Vector3(1, 0, 0);
                else if (direction == Direction.x_minus) rotate_angle = new Vector3(-1, 0, 0);
                else yield break;

                rotate_angle *= frame_par_rotate_angle;
                Group(left_parent, left_children);
                for (int i = 0; i < rotate_frame; ++i)
                {
                    parent.transform.Rotate(rotate_angle, Space.World);
                    yield return null;
                }
                Ungroup(left_parent);
            }
            else if (select == Select.center)
            {
                GameObject parent = GetCubeByPosition(center_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.x_plus) rotate_angle = new Vector3(1, 0, 0);
                else if (direction == Direction.x_minus) rotate_angle = new Vector3(-1, 0, 0);
                else yield break;

                rotate_angle *= frame_par_rotate_angle;
                Group(center_parent, center_xchildren);
                for (int i = 0; i < rotate_frame; ++i)
                {
                    parent.transform.Rotate(rotate_angle, Space.World);
                    yield return null;
                }
                Ungroup(center_parent);
            }
            else if (select == Select.right)
            {
                GameObject parent = GetCubeByPosition(right_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.x_plus) rotate_angle = new Vector3(1, 0, 0);
                else if (direction == Direction.x_minus) rotate_angle = new Vector3(-1, 0, 0);
                else yield break;

                rotate_angle *= frame_par_rotate_angle;
                Group(right_parent, right_children);
                for (int i = 0; i < rotate_frame; ++i)
                {
                    parent.transform.Rotate(rotate_angle, Space.World);
                    yield return null;
                }
                Ungroup(right_parent);
            }
        }
    }


    private IEnumerator CubeRotate()
    {
        if (direction == Direction.non) yield break;
        if (state != State.cube) yield break;

        Vector3 rotate_angle = Vector3.zero;
        if (direction == Direction.x_plus) rotate_angle = Vector3.right;
        else if (direction == Direction.x_minus) rotate_angle = Vector3.left;
        if (direction == Direction.y_plus) rotate_angle = Vector3.up;
        else if (direction == Direction.y_minus) rotate_angle = Vector3.down;
        if (direction == Direction.z_plus) rotate_angle = Vector3.forward;
        else if (direction == Direction.z_minus) rotate_angle = Vector3.back;
        rotate_angle *= frame_par_rotate_angle;
        for (int i = 0; i < rotate_frame; ++i)
        {
            cubes.transform.Rotate(rotate_angle, Space.World);
            yield return null;
        }
        direction = Direction.non;
        yield break;
    }

    private void Rotate90()
    {
        if (direction == Direction.non) return;
        if (state != State.rotate && state != State.setup) return;


        if (direction == Direction.y_plus || direction == Direction.y_minus)
        {
            if (select == Select.top)
            {
                GameObject parent = GetCubeByPosition(top_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.y_plus) rotate_angle = new Vector3(0, 1, 0);
                else if (direction == Direction.y_minus) rotate_angle = new Vector3(0, -1, 0);
                else return;

                rotate_angle *= 90.0f;
                Group(top_parent, top_children);
                parent.transform.Rotate(rotate_angle, Space.World);
                Ungroup(top_parent);
            }
            else if (select == Select.center)
            {
                GameObject parent = GetCubeByPosition(center_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.y_plus) rotate_angle = new Vector3(0, 1, 0);
                else if (direction == Direction.y_minus) rotate_angle = new Vector3(0, -1, 0);
                else return;

                rotate_angle *= 90.0f;
                Group(center_parent, center_ychildren);
                parent.transform.Rotate(rotate_angle, Space.World);
                Ungroup(center_parent);
            }
            else if (select == Select.bottom)
            {
                GameObject parent = GetCubeByPosition(bottom_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.y_plus) rotate_angle = new Vector3(0, 1, 0);
                else if (direction == Direction.y_minus) rotate_angle = new Vector3(0, -1, 0);
                else return;

                rotate_angle *= 90.0f;
                Group(bottom_parent, bottom_children);
                parent.transform.Rotate(rotate_angle, Space.World);
                Ungroup(bottom_parent);
            }
        }
        else if (direction == Direction.z_plus || direction == Direction.z_minus)
        {
            if (select == Select.front)
            {
                GameObject parent = GetCubeByPosition(front_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.z_plus) rotate_angle = new Vector3(0, 0, 1);
                else if (direction == Direction.z_minus) rotate_angle = new Vector3(0, 0, -1);
                else return;

                rotate_angle *= 90.0f;
                Group(front_parent, front_children);
                parent.transform.Rotate(rotate_angle, Space.World);
                Ungroup(front_parent);
            }
            else if (select == Select.center)
            {
                GameObject parent = GetCubeByPosition(center_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.z_plus) rotate_angle = new Vector3(0, 0, 1);
                else if (direction == Direction.z_minus) rotate_angle = new Vector3(0, 0, -1);
                else return;

                rotate_angle *= 90.0f;
                Group(center_parent, center_zchildren);
                parent.transform.Rotate(rotate_angle, Space.World);
                Ungroup(center_parent);
            }
            else if (select == Select.back)
            {
                GameObject parent = GetCubeByPosition(back_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.z_plus) rotate_angle = new Vector3(0, 0, 1);
                else if (direction == Direction.z_minus) rotate_angle = new Vector3(0, 0, -1);
                else return;

                rotate_angle *= 90.0f;
                Group(back_parent, back_children);
                parent.transform.Rotate(rotate_angle, Space.World);
                Ungroup(back_parent);
            }
        }
        else if (direction == Direction.x_plus || direction == Direction.x_minus)
        {
            if (select == Select.left)
            {
                GameObject parent = GetCubeByPosition(left_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.x_plus) rotate_angle = new Vector3(1, 0, 0);
                else if (direction == Direction.x_minus) rotate_angle = new Vector3(-1, 0, 0);
                else return;

                rotate_angle *= 90.0f;
                Group(left_parent, left_children);
                parent.transform.Rotate(rotate_angle, Space.World);
                Ungroup(left_parent);
            }
            else if (select == Select.center)
            {
                GameObject parent = GetCubeByPosition(center_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.x_plus) rotate_angle = new Vector3(1, 0, 0);
                else if (direction == Direction.x_minus) rotate_angle = new Vector3(-1, 0, 0);
                else return;

                rotate_angle *= 90.0f;
                Group(center_parent, center_xchildren);
                parent.transform.Rotate(rotate_angle, Space.World);
                Ungroup(center_parent);
            }
            else if (select == Select.right)
            {
                GameObject parent = GetCubeByPosition(right_parent);
                Vector3 rotate_angle = Vector3.zero;

                if (direction == Direction.x_plus) rotate_angle = new Vector3(1, 0, 0);
                else if (direction == Direction.x_minus) rotate_angle = new Vector3(-1, 0, 0);
                else return;

                rotate_angle *= 90.0f;
                Group(right_parent, right_children);
                parent.transform.Rotate(rotate_angle, Space.World);
                Ungroup(right_parent);
            }
        }
    }

    private void CubeRotate90()
    {
        if (direction == Direction.non) return;
        if (state != State.cube) return;

        Vector3 rotate_angle = Vector3.zero;
        if (direction == Direction.x_plus) rotate_angle = new Vector3(1, 0, 0);
        else if (direction == Direction.x_minus) rotate_angle = new Vector3(-1, 0, 0);
        if (direction == Direction.y_plus) rotate_angle = new Vector3(0, 1, 0);
        else if (direction == Direction.y_minus) rotate_angle = new Vector3(0, -1, 0);
        if (direction == Direction.z_plus) rotate_angle = new Vector3(0, 0, 1);
        else if (direction == Direction.z_minus) rotate_angle = new Vector3(0, 0, -1);
        rotate_angle *= 90.0f;
        cubes.transform.Rotate(rotate_angle, Space.World);
    }

    private void Group(Vector3 parent_position, Vector3[] children_position)
    {
        GameObject parent = GetCubeByPosition(parent_position);
        GameObject[] children = new GameObject[children_position.Length];

        {
            int i = 0;
            foreach (Vector3 vec3 in children_position)
            {
                children[i] = GetCubeByPosition(vec3);
                ++i;
            }
        }

        foreach (GameObject child in children)
        {
            child.transform.parent = parent.transform;
        }
    }


    private void Ungroup(Vector3 parent_position)
    {
        GameObject parent = GetCubeByPosition(parent_position);
        parent.transform.DetachChildren();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("cube"))
        {
            obj.transform.parent = cubes.transform;
        }
        GameObject.Find("/Top").transform.parent = parent.transform;
        GameObject.Find("/Bottom").transform.parent = parent.transform;
        GameObject.Find("/Front").transform.parent = parent.transform;
        GameObject.Find("/Back").transform.parent = parent.transform;
        GameObject.Find("/Left").transform.parent = parent.transform;
        GameObject.Find("/Right").transform.parent = parent.transform;
    }

    private void Shuffle()
    {
        if (state != State.setup) return;
        for (int i = 0; i < shuffle_count; ++i)
        {
            direction = (Direction)Random.Range(1, 7);
            select = (Select)Random.Range(0, 7);
            Rotate90();
        }
        state = State.rotate;
    }

    private GameObject GetCubeByPosition(Vector3 vec3)
    {
        GameObject return_obj = null;
        foreach (GameObject obj in cube)
        {
            Vector3 pos = new Vector3(
                Mathf.RoundToInt(obj.transform.position.x),
                Mathf.RoundToInt(obj.transform.position.y),
                Mathf.RoundToInt(obj.transform.position.z));
            if (pos == vec3) return_obj = obj;
        }
        return return_obj;
    }

}
