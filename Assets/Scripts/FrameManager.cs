using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameManager : MonoBehaviour
{
    [SerializeField]
    private Block[] grid = { 
        null, null, null, null,
        null, null, null, null,
        null, null, null, null,
        null, null, null, null
    };
    [SerializeField]
    private GameObject[] parent =
    {
        null, null, null, null,
        null, null, null, null,
        null, null, null, null,
        null, null, null, null
    };

    private int remainCount = 16;
    private bool isHot = false;
    private IEnumerator GenBlock()
    {
        yield return new WaitForSeconds(0.5f);
        Random.InitState((int)System.DateTime.Now.Ticks);
        int count = isHot ? Random.Range(0, remainCount < 4 ? remainCount + 1 : 4) : 4;
        for (int i = 0; i < count; i++)
        {
            int pivot = Random.Range(0, remainCount);
            Debug.Log("GenBlock Position Pivot: " + pivot);
            int tmpCount = 0;
            for (int j = 0; j < 16; j++)
            {
                if (grid[j] == null)
                {
                    if (tmpCount == pivot)
                    {
                        var obj = ObjectPool.GetObject();
                        obj.transform.SetParent(parent[j].transform);
                        obj.transform.localPosition = Vector3.zero;
                        obj.Init();
                        grid[j] = obj;
                        remainCount -= 1;
                    }
                    tmpCount += 1;
                }
            }
        }
        isHot = false;
    }

    void Start()
    {
        StartCoroutine(GenBlock());
    }
    void Update()
    {
        if (isHot) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            isHot = true;
            Debug.Log("up");
            MoveUp();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            isHot = true;
            Debug.Log("down");
            MoveDown();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            isHot = true;
            Debug.Log("right");
            MoveRight();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            isHot = true;
            Debug.Log("left");
            MoveLeft();
        }
    }

    void MoveUp()
    {
        bool moveFlag = false;
        for (int col = 0; col < 4; col++)
        {
            int[] movingPos = { 0, 1, 2, 3 }, movingCount = { 0, 0, 0, 0 };
            int curPivot = 0, curNum = 0;
            for (int row = 0; row < 4; row++)
            {
                int idx = row * 4 + col;
                if (grid[idx] != null)
                {
                    while (curPivot < 4) 
                    {
                        if (row == curPivot)
                        {
                            movingCount[curPivot] += 1;
                            curNum = grid[idx].GetNum();
                            break;
                        }

                        // 이동할 수 있음
                        if (movingCount[curPivot] == 0)
                        {
                            movingPos[row] = curPivot;
                            movingCount[curPivot] += 1;
                            curNum = grid[idx].GetNum();

                            // Move
                            grid[idx].Move(parent[movingPos[row] * 4 + col]);
                            grid[movingPos[row] * 4 + col] = grid[idx];
                            grid[idx] = null;
                            moveFlag = true;
                            break;
                        }
                        // 이미 블록이 있는 경우
                        else
                        {
                            // Merge 가능한가?
                            if (grid[idx].GetNum() == curNum)
                            {
                                movingPos[row] = curPivot;
                                movingCount[curPivot] += 1;
                                curPivot += 1;
                                curNum = 0;

                                // Move
                                grid[idx].Move(parent[movingPos[row] * 4 + col]);
                                grid[idx].Dead();
                                grid[idx] = null;
                                moveFlag = true;
                                // Merge
                                grid[movingPos[row] * 4 + col].Merge();
                                remainCount += 1;
                                break;
                            }
                            // Merge 불가능하면 pivot 이동
                            else
                            {
                                curPivot += 1;
                            }
                        }
                    }
                    
                }
            }
        }

        // todo Game Over
        if (remainCount == 0)
        {
            Debug.Log("Game Over!!!");
            return;
        }
        if (moveFlag)
            StartCoroutine(GenBlock());
        else
            isHot = false;
    }

    void MoveDown()
    {
        bool moveFlag = false;
        for (int col = 3; col >= 0; col--)
        {
            int[] movingPos = { 0, 1, 2, 3 }, movingCount = { 0, 0, 0, 0 };
            int curPivot = 3, curNum = 0;
            for (int row = 3; row >= 0; row--)
            {
                int idx = row * 4 + col;
                if (grid[idx] != null)
                {
                    while (curPivot >= 0)
                    {
                        if (row == curPivot)
                        {
                            movingCount[curPivot] += 1;
                            curNum = grid[idx].GetNum();
                            break;
                        }

                        // 이동할 수 있음
                        if (movingCount[curPivot] == 0)
                        {
                            movingPos[row] = curPivot;
                            movingCount[curPivot] += 1;
                            curNum = grid[idx].GetNum();

                            // Move
                            grid[idx].Move(parent[movingPos[row] * 4 + col]);
                            grid[movingPos[row] * 4 + col] = grid[idx];
                            grid[idx] = null;
                            moveFlag = true;
                            break;
                        }
                        // 이미 블록이 있는 경우
                        else
                        {
                            // Merge 가능한가?
                            if (grid[idx].GetNum() == curNum)
                            {
                                movingPos[row] = curPivot;
                                movingCount[curPivot] += 1;
                                curPivot -= 1;
                                curNum = 0;

                                // Move
                                grid[idx].Move(parent[movingPos[row] * 4 + col]);
                                grid[idx].Dead();
                                grid[idx] = null;
                                moveFlag = true;
                                // Merge
                                grid[movingPos[row] * 4 + col].Merge();
                                remainCount += 1;
                                break;
                            }
                            // Merge 불가능하면 pivot 이동
                            else
                            {
                                curPivot -= 1;
                            }
                        }
                    }

                }
            }
        }

        // todo Game Over
        if (remainCount == 0)
        {
            Debug.Log("Game Over!!!");
            return;
        }
        if (moveFlag)
            StartCoroutine(GenBlock());
        else
            isHot = false;
    }

    void MoveLeft()
    {
        bool moveFlag = false;
        for (int row = 0; row < 4; row++)
        {
            int[] movingPos = { 0, 1, 2, 3 }, movingCount = { 0, 0, 0, 0 };
            int curPivot = 0, curNum = 0;
            for (int col = 0; col < 4; col++)
            {
                int idx = row * 4 + col;
                if (grid[idx] != null)
                {
                    while (curPivot < 4)
                    {
                        if (col == curPivot)
                        {
                            movingCount[curPivot] += 1;
                            curNum = grid[idx].GetNum();
                            break;
                        }

                        // 이동할 수 있음
                        if (movingCount[curPivot] == 0)
                        {
                            movingPos[col] = curPivot;
                            movingCount[curPivot] += 1;
                            curNum = grid[idx].GetNum();

                            // Move
                            grid[idx].Move(parent[row * 4 + movingPos[col]]);
                            grid[row * 4 + movingPos[col]] = grid[idx];
                            grid[idx] = null;
                            moveFlag = true;
                            break;
                        }
                        // 이미 블록이 있는 경우
                        else
                        {
                            // Merge 가능한가?
                            if (grid[idx].GetNum() == curNum)
                            {
                                movingPos[col] = curPivot;
                                movingCount[curPivot] += 1;
                                curPivot += 1;
                                curNum = 0;

                                // Move
                                grid[idx].Move(parent[row * 4 + movingPos[col]]);
                                grid[idx].Dead();
                                grid[idx] = null;
                                moveFlag = true;
                                // Merge
                                grid[row * 4 + movingPos[col]].Merge();
                                remainCount += 1;
                                break;
                            }
                            // Merge 불가능하면 pivot 이동
                            else
                            {
                                curPivot += 1;
                            }
                        }
                    }

                }
            }
        }

        // todo Game Over
        if (remainCount == 0)
        {
            Debug.Log("Game Over!!!");
            return;
        }
        if (moveFlag)
            StartCoroutine(GenBlock());
        else
            isHot = false;
    }

    void MoveRight()
    {
        bool moveFlag = false;
        for (int row = 3; row >= 0; row--)
        {
            int[] movingPos = { 0, 1, 2, 3 }, movingCount = { 0, 0, 0, 0 };
            int curPivot = 3, curNum = 0;
            for (int col = 3; col >= 0; col--)
            {
                int idx = row * 4 + col;
                if (grid[idx] != null)
                {
                    while (curPivot >= 0)
                    {
                        if (col == curPivot)
                        {
                            movingCount[curPivot] += 1;
                            curNum = grid[idx].GetNum();
                            break;
                        }

                        // 이동할 수 있음
                        if (movingCount[curPivot] == 0)
                        {
                            movingPos[col] = curPivot;
                            movingCount[curPivot] += 1;
                            curNum = grid[idx].GetNum();

                            // Move
                            grid[idx].Move(parent[row * 4 + movingPos[col]]);
                            grid[row * 4 + movingPos[col]] = grid[idx];
                            grid[idx] = null;
                            moveFlag = true;
                            break;
                        }
                        // 이미 블록이 있는 경우
                        else
                        {
                            // Merge 가능한가?
                            if (grid[idx].GetNum() == curNum)
                            {
                                movingPos[col] = curPivot;
                                movingCount[curPivot] += 1;
                                curPivot -= 1;
                                curNum = 0;

                                // Move
                                grid[idx].Move(parent[row * 4 + movingPos[col]]);
                                grid[idx].Dead();
                                grid[idx] = null;
                                moveFlag = true;
                                // Merge
                                grid[row * 4 + movingPos[col]].Merge();
                                remainCount += 1;
                                break;
                            }
                            // Merge 불가능하면 pivot 이동
                            else
                            {
                                curPivot -= 1;
                            }
                        }
                    }

                }
            }
        }

        // todo Game Over
        if (remainCount == 0)
        {
            Debug.Log("Game Over!!!");
            return;
        }
        if (moveFlag)
            StartCoroutine(GenBlock());
        else
            isHot = false;
    }
}
