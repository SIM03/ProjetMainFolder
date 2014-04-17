using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TOOLS
{
    public class ScreenNotification : ScreenMessage
    {
        const float FADE_OUT_FPS = 1f / 60;

        float NotificationTimeInSeconds { get; set; }
        float TotalSecondsWhenVisible { get; set; }
        float TotalSecondsWhenFadeStart { get; set; }
        float ElapsedTimeSinceVisible { get; set; }

        float FadeOutSeconds { get; set; }
        float Transparency { get; set; }

        public ScreenNotification(Game game, CaméraSubjectivePhysique gameCamera, string title, string parameter, string font, float offsetx, float offsety, float intervalleMAJ, float notificationTimeInSeconds)
            : base(game, gameCamera, title, parameter, font, offsetx, offsety, intervalleMAJ)
        {
            NotificationTimeInSeconds = notificationTimeInSeconds;
        }

        public override void Initialize()
        {
            ElapsedTimeSinceVisible = 0;
            TotalSecondsWhenVisible = 0;
            TotalSecondsWhenFadeStart = 0;
            Transparency = 1;  // opaque
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {

            float timelapse = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ElapsedTimeSinceUp += timelapse;
            if (ElapsedTimeSinceUp >= IntervalleMAJ)
            {
                WaitForNotification(gameTime);
                GetMessage();
                ElapsedTimeSinceUp = 0;
            }

        }

        private void WaitForNotification(GameTime gameTime)
        {
            if (Visible)
            {
                if (TotalSecondsWhenVisible == 0)
                    TotalSecondsWhenVisible = (float)gameTime.TotalGameTime.TotalSeconds;

                if (IsReadyToFadeOut(gameTime))
                    FadeOut();
            }
        }

        private void FadeOut()
        {
            FontColor *= Transparency;
            if (Transparency >= 0)
            {
                FadeOutSeconds += FADE_OUT_FPS;
            }
            else
            {
                TotalSecondsWhenVisible = 0;
                Transparency = 1;
                FontColor = Color.White;
                this.Visible = false;
            }
            Transparency = Transparency - 0.01f;
        }

        private bool IsReadyToFadeOut(GameTime gameTime)
        {
            return ((gameTime.TotalGameTime.TotalSeconds >= TotalSecondsWhenVisible + NotificationTimeInSeconds)
                && (gameTime.TotalGameTime.TotalSeconds <= TotalSecondsWhenVisible + NotificationTimeInSeconds + FadeOutSeconds + 0.1f));
        }
    }
}
