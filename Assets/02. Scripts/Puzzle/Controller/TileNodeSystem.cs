using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNodeSystem : MonoBehaviour
{
    [SerializeField] private TileNode[] _tileNodes;
    private TileNode[,] _tileNodeGrid = new TileNode[7, 7];
    public TileNode[,] TileNodeGrid => _tileNodeGrid;
    private const int GridSize = 7;
    private bool _isMoving = false;

    private void Awake()
    {
        for (int y = 0; y < GridSize; y++)
        {
            for (int x = 0; x < GridSize; x++)
            {
                _tileNodeGrid[y, x] = _tileNodes[GridSize * y + x];
            }
        }
    }

    public void Action(InputKey inputKey)
    {
        if (_isMoving) return; // ���� �̵��� ������ �ʾ����� �Է��� ����

        if (inputKey == InputKey.Left || inputKey == InputKey.Right)
        {
            for (int y = 0; y < GridSize; y++)
            {
                List<TileNode> line = new List<TileNode>();
                for (int x = 0; x < GridSize; x++)
                {
                    line.Add(_tileNodeGrid[y, x]);
                }
                StartCoroutine(ProcessLine(line, inputKey));
            }
        }
        else if (inputKey == InputKey.Up || inputKey == InputKey.Down)
        {
            for (int x = 0; x < GridSize; x++)
            {
                List<TileNode> line = new List<TileNode>();
                for (int y = 0; y < GridSize; y++)
                {
                    line.Add(_tileNodeGrid[y, x]);
                }
                StartCoroutine(ProcessLine(line, inputKey));
            }
        }
    }

    private IEnumerator ProcessLine(List<TileNode> line, InputKey key)
    {
        _isMoving = true;

        // 1. �ش� �ٿ��� ������ ����
        List<Mana> manaInLine = new List<Mana>();
        foreach (var node in line)
        {
            if (node.OnMana)
            {
                manaInLine.Add(node.CurrentMana);
                node.ClearMana();
            }
        }

        if (manaInLine.Count == 0)
        {
            _isMoving = false;
            yield break; // ó���� ������ ������ ����
        }

        // 2. ���� ��ġ��
        if (key == InputKey.Right || key == InputKey.Down) manaInLine.Reverse();

        List<Mana> mergedMana = new List<Mana>();
        for (int i = 0; i < manaInLine.Count; i++)
        {
            if (i + 1 < manaInLine.Count && manaInLine[i].ManaLevel == manaInLine[i + 1].ManaLevel && manaInLine[i].ManaLevel < 3)
            {
                manaInLine[i].SetLevel(manaInLine[i].ManaLevel + 1);
                mergedMana.Add(manaInLine[i]);
                Destroy(manaInLine[i + 1].gameObject);
                i++;
            }
            else
            {
                mergedMana.Add(manaInLine[i]);
            }
        }

        // 3. ������ ���� ���� (Protected Index) ã��
        int protectedIndex = -1;
        if (key == InputKey.Right || key == InputKey.Down)
        {
            // ������/�Ʒ��� �̵�: ���������� ��ֹ� ���� �� ã��
            for (int i = line.Count - 1; i >= 0; i--)
            {
                if (line[i].TileState != ETileState.Melt && line[i].TileState != ETileState.Destroy)
                {
                    protectedIndex = i;
                    break;
                }
            }
        }
        else // ����/���� �̵�
        {
            for (int i = 0; i < line.Count; i++)
            {
                if (line[i].TileState != ETileState.Melt && line[i].TileState != ETileState.Destroy)
                {
                    protectedIndex = i;
                    break;
                }
            }
        }

        if (protectedIndex == -1) // �̵��� ������ ���� ����
        {
            // ���� ��ġ�� ���� ���� (�ʿ� ��) �Ǵ� �ı�
            foreach (var mana in mergedMana) Destroy(mana.gameObject);
            _isMoving = false;
            yield break;
        }

        // 4. ������ ������ protectedIndex���� ��ġ �� �̵�
        if (key == InputKey.Right || key == InputKey.Down)
        {
            for (int i = 0; i < mergedMana.Count; i++)
            {
                int destinationIndex = protectedIndex - i;
                if (destinationIndex < 0) break; // ������ �����ϸ� ����
                StartCoroutine(TransferMana(mergedMana[i], line[destinationIndex]));
            }
        }
        else // ����/���� �̵�
        {
            for (int i = 0; i < mergedMana.Count; i++)
            {
                int destinationIndex = protectedIndex + i;
                if (destinationIndex >= line.Count) break;
                StartCoroutine(TransferMana(mergedMana[i], line[destinationIndex]));
            }
        }

        // ��� �̵� �ִϸ��̼��� ���� ������ ��� (�����ϰ� �ִ� �̵��ð���ŭ ���)
        yield return new WaitForSeconds(0.2f);
        _isMoving = false;
    }

    private IEnumerator TransferMana(Mana mana, TileNode endTileNode)
    {
        if (mana == null) yield break;

        float moveDuration = 0.15f;
        Vector3 startPosition = mana.transform.position;
        Vector3 endPosition = endTileNode.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            mana.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mana.transform.position = endPosition;
        endTileNode.ReceiveMana(mana);
    }
}