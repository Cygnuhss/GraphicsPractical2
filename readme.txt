Graphics Practical 2
02-06-2015

Yoni Groosman - 4101421
Jelmer van Nuss - 4058925


Assignments:

1.1 Coloring using normals 			- Done
1.1 Checkerboard pattern 			- Done
2.1 Lambertian shading 				- Done (using a directional light)
2.2 Ambient shading 				- Done
2.3 Phong shading 				- Done (using Blinn-Phong shading)
2.4 Non-uniform scaling problem 		- Done
3.1 Texturing a quad using UV-coordinates 	- Not done

Bonus assignments:

4.1 Gamma correction				- Done

See the shader file PostProcessing.fx and the class file Game1.cs for this
implementation. Instead of rendering directly to the screen, the scene is first
rendered to a render target, which is then post-processed with gamma correction
and displayed to the screen.
The gamma value is set to 1.0 at default and is applied to all the other assignments.
With the default value, however, this will not change the output.

4.2 Normal mapping				- Not done