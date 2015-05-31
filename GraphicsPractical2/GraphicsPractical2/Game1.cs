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

        // Quad texture
        private Texture2D quadTexture;
        // Quad normal map
        private Texture2D quadNormalMap;

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
            this.modelMaterial.SpecularPower = 2.50f;
            // Do not set a texture for the model.
            this.modelMaterial.DiffuseTexture = null;
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
            // Top right
            this.quadVertices[1].Position = new Vector3(1, 0, -1);
            this.quadVertices[1].Normal = quadNormal;
            // Bottom left
            this.quadVertices[2].Position = new Vector3(-1, 0, 1);
            this.quadVertices[2].Normal = quadNormal;
            // Bottom right
            this.quadVertices[3].Position = new Vector3(1, 0, 1);
            this.quadVertices[3].Normal = quadNormal;

            this.quadIndices = new short[] { 0, 1, 2, 1, 2, 3 };
            this.quadTransform = Matrix.CreateScale(scale);

            // Setup the quad texture.
            this.quadTexture = this.Content.Load<Texture2D>("Textures/CobblestonesDiffuse");
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
            // Clear the screen in a predetermined color and clear the depth buffer
            this.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DeepSkyBlue, 1.0f, 0);

            // Get the model's only mesh
            ModelMesh mesh = this.model.Meshes[0];
            Effect effect = mesh.Effects[0];

            // Set the effect parameters
            effect.CurrentTechnique = effect.Techniques["Simple"];
            // Matrices for 3D perspective projection
            this.camera.SetEffectParameters(effect);
            Matrix world = Matrix.CreateScale(10.0f);
            effect.Parameters["World"].SetValue(world);
            // Set world inverse transpose.
            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
            effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
            // Set the light source.
            effect.Parameters["LightSourceDirection"].SetValue(new Vector3(-1.0f, -1.0f, -1.0f));
            // Set the view direction.
            Vector3 view = new Vector3(this.camera.ViewMatrix.M13, this.camera.ViewMatrix.M23, this.camera.ViewMatrix.M33);
            effect.Parameters["ViewVector"].SetValue(view);
            // Set all the material parameters.
            this.modelMaterial.SetEffectParameters(effect);
            // Draw the model
            mesh.Draw();

            base.Draw(gameTime);
        }
    }
}
