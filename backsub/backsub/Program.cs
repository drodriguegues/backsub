﻿// Released to the public domain. Use, modify and relicense at will.

using System;
using System.IO;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;

namespace BackSub
{
	class Game : GameWindow
	{
		/// <summary>Creates a 800x600 window with the specified title.</summary>
		public Game()
			: base(512, 512, GraphicsMode.Default, "OpenTK Quick Start Sample")
		{
			VSync = VSyncMode.On;
		}

		GLShader shader;
		GLTextureObject mainTexture;
		GLTextureObject renderTexture;
		GLFrameBufferObject fbo;
		/// <summary>Load resources here.</summary>
		/// <param name="e">Not used.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Texture2D);
			GL.Enable(EnableCap.CullFace);
			GL.CullFace(CullFaceMode.Back);

			this.shader = new GLShader(File.ReadAllText(GetAbsolutePath("shader.vert")), File.ReadAllText(GetAbsolutePath("shader.frag")));

			this.mainTexture = new GLTextureObject(new Bitmap(GetAbsolutePath("output0106.png")));
			this.mainTexture.TextureUnit = TextureUnit.Texture0;
			
			this.renderTexture = new GLTextureObject(new Bitmap(GetAbsolutePath("output0106.png")));
			this.renderTexture.TextureUnit = TextureUnit.Texture1;

			fbo = new GLFrameBufferObject(512, 512);
			fbo.DrawBuffer = FramebufferAttachment.ColorAttachment0;
			fbo.AttachTexture2D(FramebufferAttachment.ColorAttachment0, this.renderTexture.TextureId);
			fbo.Validate(true);
			GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // return to visible framebuffer
		}
		
		/// <summary>
		/// Gets the absolute path of a file relative to executing location.
		/// </summary>
		/// <returns>
		/// The absolute path.
		/// </returns>
		/// <param name='relpath'>
		/// Relative path 
		/// </param>
		string GetAbsolutePath(string relpath)
		{
			return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + relpath;
		}

		/// <summary>
		/// Called when your window is resized. Set your viewport here. It is also
		/// a good place to set up your projection matrix (which probably changes
		/// along when the aspect ratio of your window).
		/// </summary>
		/// <param name="e">Not used.</param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			//GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

			//Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
			Matrix4 projection = Matrix4.CreateOrthographicOffCenter(-1, 1, -1, 1, 1, 64);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref projection);
		}

		/// <summary>
		/// Called when it is time to setup the next frame. Add you game logic here.
		/// </summary>
		/// <param name="e">Contains timing information for framerate independent logic.</param>
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);

			if (Keyboard[Key.Escape])

				Exit();
		}
		/// <summary>
		/// Called when it is time to render the next frame. Add your rendering code here.
		/// </summary>
		/// <param name="e">Contains timing information.</param>
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);

			//Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
			Matrix4 modelview = Matrix4.Identity;
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modelview);

			//Render texture to texture
			GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			mainTexture.Render();
			this.shader.SetUniform("texture0", mainTexture.TextureUnit);
			this.shader.SetUniform("renderColor", 1);
			this.fbo.BeginRender();

			GL.Begin(BeginMode.Quads);

			GL.TexCoord2(0, 0);  GL.Color3(0.0f, 0.0f, 0.0f); GL.Vertex3(-0.9f, -0.9f, -4.0f);
			GL.TexCoord2(1, 0);  GL.Color3(1.0f, 0.0f, 0.0f); GL.Vertex3(0.9f, -0.9f, -4.0f);
			GL.TexCoord2(1, 1);  GL.Color3(0.0f, 1.0f, 0.0f); GL.Vertex3(0.9f, 0.9f, -4.0f);
			GL.TexCoord2(0, 1);  GL.Color3(0.0f, 0.0f, 1.0f); GL.Vertex3(-0.9f, 0.9f, -4.0f);

			GL.End();

			this.fbo.EndRender();

			//Render texture to screen
			GL.ClearColor(0.0f, 1.0f, 0.0f, 0.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			
			renderTexture.Render();
			this.shader.SetUniform("texture0", renderTexture.TextureUnit);
			this.shader.SetUniform("renderColor", 0);
			
			GL.Begin(BeginMode.Quads);

			GL.TexCoord2(0, 0);  GL.Color3(0.0f, 0.0f, 0.0f); GL.Vertex3(-0.9f, -0.9f, -4.0f);
			GL.TexCoord2(1, 0);  GL.Color3(1.0f, 0.0f, 0.0f); GL.Vertex3(0.9f, -0.9f, -4.0f);
			GL.TexCoord2(1, 1);  GL.Color3(0.0f, 1.0f, 0.0f); GL.Vertex3(0.9f, 0.9f, -4.0f);
			GL.TexCoord2(0, 1);  GL.Color3(0.0f, 0.0f, 1.0f); GL.Vertex3(-0.9f, 0.9f, -4.0f);
			
			GL.End();
			
			SwapBuffers();
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			// The 'using' idiom guarantees proper resource cleanup.
			// We request 30 UpdateFrame events per second, and unlimited
			// RenderFrame events (as fast as the computer can handle).
			using (Game game = new Game())
			{
				game.Run(30.0);
			}
		}
	}
}