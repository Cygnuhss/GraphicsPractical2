
-- Graphics Practical 2 --
	02-06-2015

Yoni Groosman 	- 4101421
Jelmer van Nuss - 4058925

-------------------------
------- Teamwork --------
-------------------------

Most of the work was done together, either in real life or through internet
communication. Jelmer did slightly more work on assignments 1.1, 2.1, 2.2 and 2.3
due to personal circumstances of Yoni. Yoni will make up for that in the next
assignment, and the division of labor did not come across as unfair.
Jelmer implemented the bonus assignment 4.1 on his own, out of curiosity.

-------------------------
------ Assignments ------
-------------------------

1.1 Coloring using normals 			- Done
1.2 Checkerboard pattern 			- Done
2.1 Lambertian shading 				- Done (using a directional light)
2.2 Ambient shading 				- Done
2.3 Phong shading 				- Done (using Blinn-Phong shading)
2.4 Non-uniform scaling problem 		- Done
3.1 Texturing a quad using UV-coordinates 	- Not finished

The shader code for this assignment is finished, we now need to render the quad
with a texture.

-------------------------
--- Bonus assignments ---
-------------------------

4.1 Gamma correction				- Done

See the shader file PostProcessing.fx and the class file Game1.cs for this
implementation. Instead of rendering directly to the screen, the scene is first
rendered to a render target, which is then post-processed with gamma correction
and displayed to the screen. This resulted in some different methods used for
drawing, which do not affect the end result with post-processing disabled.
The gamma value is set to 1.0 at default and is applied to all the other assignments.
With the default value, however, this will not change the output.

4.2 Normal mapping				- Not done

This assignment is not implemented at all.