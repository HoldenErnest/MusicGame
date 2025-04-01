// Holden Ernest - 4/1/2025
// singleton class for a null Note object

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class NullNote : Note {

    public NullNote() {

    }

    public override bool isActive() {
        return false;
    }
    public override bool isNull() {
        return true;
    }

}
