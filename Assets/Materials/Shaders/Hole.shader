Shader "Custom/Hole"
{

    SubShader
    {
        //render mask after regular geometry, but before masked geometry and transparent things
        Tags { "Queue"="Geometry-1" }

        //don't draw RGBA channels; just depth buffer
        Colormask 0
        ZWrite On

        //do nothing specific in the pass
        Pass{}

    }

}
