using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GraphicsPractical2
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Often used XNA objects
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private FrameRateCounter frameRateCounter;

        // Game objects and variables
        private Camera camera;

        // Model
        private Model model;
        private Material modelMaterial;

        // Quad
        private VertexPositionNormalTexture[] quadVertices;
        private short[] quadIndices;
        private Matrix quadTransform;

        // Quad material
        private Material quadMaterial;

        // Gamma correction
        private Effect gammaEffect;
        private RenderTarget2D renderTarget;

        public Game1()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            // Create and add a frame rate counter
            this.frameRateCounter = new FrameRateCounter(this);
            this.Components.Add(this.frameRateCounter);
        }

        protected override void Initialize()
        {
            // Copy over the device's rasterizer state to change the current fillMode
            this.GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };
            // Set up the window
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.IsFullScreen = false;
            // Let the renderer draw and update as often as possible
            this.graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
            // Flush the changes to the device parameters to the graphics card
            this.graphics.ApplyChanges();
            // Initialize the camera
            this.camera = new Camera(new Vector3(0, 50, 100), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

            this.IsMouseVisible = true;

            // Set the render target.
            renderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a SpriteBatch object.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            // Load the "Simple" effect.
            Effect effect = this.Content.Load<Effect>("Effects/Simple");
            // Load the model and let it use the "Simple" effect.
            this.model = this.Content.Load<Model>("Models/Teapot");
            this.model.Meshes[0].MeshParts[0].Effect = effect;

            // Setup the quad.
            this.setupQuad();

            // Setup the material.
            this.modelMaterial = new Material();
            // Set the ambient color.
            this.modelMaterial.AmbientColor = Color.Red;
            // Set the ambient intensity.
            this.modelMaterial.AmbientIntensity = 0.2f;
            // Set the diffuse color.
            this.modelMaterial.DiffuseColor = Color.Red;
            // Set the specular color.
            this.modelMaterial.SpecularColor = Color.White;
            // Set the specular intensity.
            this.modelMaterial.SpecularIntensity = 2.0f;
            // Set the specular power.
            this.modelMaterial.SpecularPower = 25.0f;
            // Do not set a texture for the model.
            this.modelMaterial.DiffuseTexture = null;
            // Disable the normal and procedural coloring.
            this.modelMaterial.NormalColoring = false;
            this.modelMaterial.ProceduralColoring = false;

            // Load the "PostProcessing" effect.
            gammaEffect = Content.Load<Effect>("Effects/PostProcessing");
        }

        /// <summary>
        /// Sets up a 2 by 2 quad around the origin.
        /// </summary>
        private void setupQuad()
        {
            float scale = 50.0f;

            // Normal points up
            Vector3 quadNormal = new Vector3(0, 1, 0);

            this.quadVertices = new VertexPositionNormalTexture[4];
            // Top left
            this.quadVertices[0].Position = new Vector3(-1, 0, -1);
            this.quadVertices[0].Normal = quadNormal;
            this.quadVertices[0].TextureCoordinate = new Vector2(0.0f, 0.0f);
            // Top right
            this.quadVertices[1].Position = new Vector3(1, 0, -1);
            this.quadVertices[1].Normal = quadNormal;
            this.quadVertices[1].TextureCoordinate = new Vector2(1.0f, 0.0f);
            // Bottom left
            this.quadVertices[2].Position = new Vector3(-1, 0, 1);
            this.quadVertices[2].Normal = quadNormal;
            this.quadVertices[2].TextureCoordinate = new Vector2(0.0f, 1.0f);
            // Bottom right
            this.quadVertices[3].Position = new Vector3(1, 0, 1);
            this.quadVertices[3].Normal = quadNormal;
            this.quadVertices[3].TextureCoordinate = new Vector2(1.0f, 1.0f);

            this.quadIndices = new short[] { 0, 1, 2, 1, 2, 3 };
            this.quadTransform = Matrix.CreateScale(scale);

            // Setup the material.
            this.quadMaterial = new Material();
            // Set the ambient color.
            this.quadMaterial.AmbientColor = Color.White;
            // Set the ambient intensity.
            this.quadMaterial.AmbientIntensity = 0.0f;
            // Set the diffuse color.
            this.quadMaterial.DiffuseColor = Color.White;
            // Set the specular color.
            this.quadMaterial.SpecularColor = Color.White;
            // Set the specular intensity.
            this.quadMaterial.SpecularIntensity = 0.0f;
            // Set the specular power.
            this.quadMaterial.SpecularPower = 0.0f;
            // Set the quad texture.
            this.quadMaterial.DiffuseTexture = this.Content.Load<Texture2D>("Textures/CobblestonesDiffuse");
            // Disable the normal and procedural coloring.
            this.quadMaterial.NormalColoring = false;
            this.quadMaterial.ProceduralColoring = false;
        }

        protected override void Update(GameTime gameTime)
        {
            float timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds * 60.0f;

            // Update the window title
            this.Window.Title = "XNA Renderer | FPS: " + this.frameRateCounter.FrameRate;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            DrawSceneToTexture(renderTarget);

            GraphicsDevice.Clear(Color.Black);

            // Set the gamma value.
            // A value of 1.5 is used in the screenshot for gamma correction, 1.0 is used to apply
            // no correction.
            gammaEffect.Parameters["Gamma"].SetValue(1.0f);
            // Apply gamma correction.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
                        SamplerState.LinearClamp, DepthStencilState.Default,
                        RasterizerState.CullNone, gammaEffect);

            spriteBatch.Draw(renderTarget, new Rectangle(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void DrawScene()
        {
            // Clear the screen in a predetermined color and clear the depth buffer
            this.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DeepSkyBlue, 1.0f, 0);

            // Get the model's only mesh
            ModelMesh mesh = this.model.Meshes[0];
            Effect effect = mesh.Effects[0];

            // Set the effect parameters
            effect.CurrentTechnique = effect.Techniques["Simple"];
            // Matrices for 3D perspective projection
            this.camera.SetEffectParameters(effect);
            // Non-uniform scale.
            Matrix scale = Matrix.CreateScale(new Vector3(10.0f, 6.5f, 2.5f));
            // Uniform scale.
            Matrix world = Matrix.CreateScale(10.0f);
            effect.Parameters["World"].SetValue(scale);
            // Set world inverse transpose.
            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
            effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
            // Set the light source.
            effect.Parameters["LightSourceDirection"].SetValue(new Vector3(-1.0f, -1.0f, -1.0f));
            // Set the view direction.
            Vector3 view = this.camera.Eye;
            effect.Parameters["EyePos"].SetValue(view);
            // Set all the material parameters.
            this.modelMaterial.SetEffectParameters(effect);

            // Draw the model
            mesh.Draw();
            /*
            // Set the effect parameters
            effect.CurrentTechnique = effect.Techniques["Simple"];
            // Matrices for 3D perspective projection
            this.camera.SetEffectParameters(effect);
            effect.Parameters["World"].SetValue(world);
            // Set world inverse transpose.
            worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(this.quadTransform * world));
            effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
            // Set the light source.
            effect.Parameters["LightSourceDirection"].SetValue(new Vector3(-1.0f, -1.0f, -1.0f));
            */
            // Set all the quad material parameters.
            this.quadMaterial.SetEffectParameters(effect);

            // Draw the ground texture.
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                    this.quadVertices, 0, this.quadVertices.Length,
                    this.quadIndices, 0, this.quadIndices.Length / 3);
            }
        }

        protected void DrawSceneToTexture(RenderTarget2D renderTarget)
        {
            // Set the render target.
            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            // Draw the scene.
            DrawScene();

            // Drop the render target.
            GraphicsDevice.SetRenderTarget(null);
        }
    }
}
