// Holden Ernest - 4/1/2025
// singleton class for a null Octave object
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public sealed class NullOctave : Octave {
    public NullOctave() {

    }
    public override bool isNull() {
        return true;
    }
}
