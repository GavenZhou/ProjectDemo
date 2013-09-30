using UnityEngine;
using System.Collections.Generic;

public class ObjPool<T> where T : class, new()
{
    public int size = 0;
    public int idx = 0;
    public T[] data;
    public T[] initData;

    public void Init(int _size) {
        size = _size;
        initData = new T[size];
        data = new T[size];
        for (int i = 0; i < size; ++i) {
            T obj = new T();
            initData[i] = obj;
            data[i] = initData[i];
        }
        idx = size - 1;
    }

    public void Reset() {
        for (int i = 0; i < size; ++i) {
            data[i] = initData[i];
        }
        idx = size - 1;
    }

    public T Request() {
        if (idx < 0) {
            Debug.LogError("Error: the objPool do not have enough free item.");
            return null;
        }

        T result = data[idx];
        --idx;
        return result;
    }

    public void Return(T _item) {
        ++idx;
        data[idx] = _item;
    }
}

