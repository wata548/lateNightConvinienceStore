using System.Collections;
using System.Collections.Generic;
using System;

public class Shuffler<T> : IEnumerable<T> {

    private List<T> items;
    private Random random;

    public Shuffler(IEnumerable<T> list) {

        items = new List<T>(list);
        random = new Random();
    }
    
    public IEnumerator<T> GetEnumerator() {

        for (int i = items.Count - 1; i > 0; i--) {

            int index = random.Next(0, i + 1);
            (items[i], items[index]) = (items[index], items[i]);
        }

        foreach (var item in items) {

            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}