using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct InventoryEntry
{
    public string Type;       // ex: "L_shaped_LeftTop"
    public Material Mat;      // �κ��丮�� ������ ��Ƽ���� ����

    public InventoryEntry(string type, Material mat)
    {
        Type = type;
        Mat = mat;
    }
}

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int MAX_ITEM = 8;

    public static InventoryManager Instance { get; private set; }

    // Ÿ��(Type)�� ��Ƽ����(Mat)�� �Բ� ����
    public List<InventoryEntry> items = new List<InventoryEntry>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    /// <summary>
    /// Ÿ�԰� ��Ƽ������ �޾� �κ��丮�� �߰��մϴ�.
    /// </summary>
    public bool AddItem(string type, Material mat)
    {
        if (items.Count >= MAX_ITEM)
        {
            Debug.LogWarning("[�κ��丮] ���� á���ϴ�!");
            return false;
        }

        items.Add(new InventoryEntry(type, mat));
        Debug.Log($"[�κ��丮] �߰���: {type}");
        return true;
    }

    /// <summary>
    /// �κ��丮���� �ش� �ε����� �׸��� �����մϴ�.
    /// </summary>
    public void RemoveAt(int index)
    {
        if (index >= 0 && index < items.Count)
            items.RemoveAt(index);
    }

    /// <summary>
    /// ���� �κ��丮�� ��� �����Դϴ�.
    /// </summary>
    public int Count => items.Count;
}
