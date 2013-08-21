using UnityEngine;
using System.Collections;

public static class Formulas
{
	public static Vector3 GetQuadraticCoordinates(float percent, Vector3 iniPos, Vector3 midPos, Vector3 endPos)
	{
		return (1f - percent) * (1f - percent) * iniPos + 2 * percent * (1 - percent) * midPos + percent * percent * endPos;
	}
	
	public static float AngleAroundAxis (Vector3 dirA, Vector3 dirB, Vector3 axis)
	{
	    // Project A and B onto the plane orthogonal target axis
	    dirA = dirA - Vector3.Project (dirA, axis);
	    dirB = dirB - Vector3.Project (dirB, axis);
	   
	    // Find (positive) angle between A and B
	    float angle = Vector3.Angle (dirA, dirB);
	   
	    // Return angle multiplied with 1 or -1
	    return angle * (Vector3.Dot (axis, Vector3.Cross (dirA, dirB)) < 0 ? -1 : 1);
	}
}
