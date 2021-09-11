using System;
using System.Collections.Generic;
using YoonFactory.Image;

namespace YoonFactory
{
    public class YoonObject : IYoonParameter, IEquatable<YoonObject>
    {
        #region Supported IDisposable Pattern

        ~YoonObject()
        {
            this.Dispose(false);
        }

        private bool _disposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed) return;
            if (disposing)
            {
                //
            }

            this._disposed = true;
        }

        #endregion

        private const int DEFAULT_LABEL = 0;
        private const int DEFAULT_PIX_COUNT = 0;
        private const double DEFAULT_SCORE = 0.0;

        public int Label { get; set; } = DEFAULT_LABEL;
        public double Score { get; set; } = DEFAULT_SCORE;
        public IYoonFigure Feature { get; set; }
        public IYoonVector Position { get; set; }
        public int PixelCount { get; set; } = DEFAULT_PIX_COUNT;

        public YoonObject()
        {
            Label = DEFAULT_LABEL;
            Score = DEFAULT_SCORE;
            PixelCount = DEFAULT_PIX_COUNT;
            switch (Feature)
            {
                case YoonRect2N:
                    Feature = new YoonRect2N();
                    Position = new YoonVector2N();
                    break;
                case YoonRect2D:
                    Feature = new YoonRect2D();
                    Position = new YoonVector2D();
                    break;
                case YoonRectAffine2D:
                    Feature = new YoonRectAffine2D();
                    Position = new YoonVector2D();
                    break;
                case YoonLine2N:
                    Feature = new YoonLine2N();
                    Position = new YoonVector2N();
                    break;
                case YoonLine2D:
                    Feature = new YoonLine2D();
                    Position = new YoonVector2D();
                    break;
                case YoonVector2N:
                    Feature = new YoonVector2N();
                    Position = new YoonVector2N();
                    break;
                case YoonVector2D:
                    Feature = new YoonVector2D();
                    Position = new YoonVector2D();
                    break;
                default:
                    Feature = new YoonRect2N();
                    Position = new YoonVector2N();
                    break;
            }
        }

        public YoonObject(int nLabel, IYoonFigure pFeature)
        {
            Label = nLabel;
            Score = DEFAULT_SCORE;
            PixelCount = DEFAULT_PIX_COUNT;
            switch (pFeature)
            {
                case YoonRect2N pRect2N:
                    Feature = pRect2N.Clone();
                    Position = pRect2N.CenterPos.Clone();
                    break;
                case YoonRect2D pRect2D:
                    Feature = pRect2D.Clone();
                    Position = pRect2D.CenterPos.Clone();
                    break;
                case YoonRectAffine2D pRectAffine2D:
                    Feature = pRectAffine2D.Clone();
                    Position = pRectAffine2D.CenterPos.Clone();
                    break;
                case YoonLine2N pLine2N:
                    Feature = pLine2N.Clone();
                    Position = pLine2N.CenterPos.Clone();
                    break;
                case YoonLine2D pLine2D:
                    Feature = pLine2D.Clone();
                    Position = pLine2D.CenterPos.Clone();
                    break;
                case YoonVector2N pVector2N:
                    Feature = pVector2N.Clone();
                    Position = pVector2N.Clone();
                    break;
                case YoonVector2D pVector2D:
                    Feature = pVector2D.Clone();
                    Position = pVector2D.Clone();
                    break;
                default:
                    throw new FormatException("[YOONIMAGE EXCEPTION] Object format is not correct");
            }
        }

        public YoonObject(int nLabel, IYoonFigure pFeature, IYoonVector pPosCurrent)
        {
            Label = nLabel;
            Score = DEFAULT_SCORE;
            PixelCount = DEFAULT_PIX_COUNT;
            Position = pPosCurrent.Clone();
            switch (pFeature)
            {
                case IYoonRect pRect:
                    Feature = pRect.Clone();
                    break;
                case IYoonLine pLine:
                    Feature = pLine.Clone();
                    break;
                case IYoonVector pVector:
                    Feature = pVector.Clone();
                    break;
                default:
                    throw new FormatException("[YOONIMAGE EXCEPTION] Object format is not correct");
            }
        }

        public YoonObject(int nLabel, IYoonFigure pFeature, int nCount)
        {
            Label = nLabel;
            Score = DEFAULT_SCORE;
            PixelCount = nCount;
            switch (pFeature)
            {
                case YoonRect2N pRect2N:
                    Feature = pRect2N.Clone();
                    Position = pRect2N.CenterPos.Clone();
                    break;
                case YoonRect2D pRect2D:
                    Feature = pRect2D.Clone();
                    Position = pRect2D.CenterPos.Clone();
                    break;
                case YoonRectAffine2D pRectAffine2D:
                    Feature = pRectAffine2D.Clone();
                    Position = pRectAffine2D.CenterPos.Clone();
                    break;
                case YoonLine2N pLine2N:
                    Feature = pLine2N.Clone();
                    Position = pLine2N.CenterPos.Clone();
                    break;
                case YoonLine2D pLine2D:
                    Feature = pLine2D.Clone();
                    Position = pLine2D.CenterPos.Clone();
                    break;
                case YoonVector2N pVector2N:
                    Feature = pVector2N.Clone();
                    Position = pVector2N.Clone();
                    break;
                case YoonVector2D pVector2D:
                    Feature = pVector2D.Clone();
                    Position = pVector2D.Clone();
                    break;
                default:
                    throw new FormatException("[YOONIMAGE EXCEPTION] Object format is not correct");
            }
        }

        public YoonObject(int nLabel, IYoonFigure pFeature, IYoonVector pPosCurrent, int nCount)
        {
            Label = nLabel;
            Score = DEFAULT_SCORE;
            PixelCount = nCount;
            Position = pPosCurrent.Clone();
            switch (pFeature)
            {
                case IYoonRect pRect:
                    Feature = pRect.Clone();
                    break;
                case IYoonLine pLine:
                    Feature = pLine.Clone();
                    break;
                case IYoonVector pVector:
                    Feature = pVector.Clone();
                    break;
                default:
                    throw new FormatException("[YOONIMAGE EXCEPTION] Object format is not correct");
            }
        }

        public YoonObject(int nLabel, IYoonFigure pFeature, double dScore, int nCount)
        {
            Label = nLabel;
            Score = dScore;
            PixelCount = nCount;
            switch (pFeature)
            {
                case YoonRect2N pRect2N:
                    Feature = pRect2N.Clone();
                    Position = pRect2N.CenterPos.Clone();
                    break;
                case YoonRect2D pRect2D:
                    Feature = pRect2D.Clone();
                    Position = pRect2D.CenterPos.Clone();
                    break;
                case YoonRectAffine2D pRectAffine2D:
                    Feature = pRectAffine2D.Clone();
                    Position = pRectAffine2D.CenterPos.Clone();
                    break;
                case YoonLine2N pLine2N:
                    Feature = pLine2N.Clone();
                    Position = pLine2N.CenterPos.Clone();
                    break;
                case YoonLine2D pLine2D:
                    Feature = pLine2D.Clone();
                    Position = pLine2D.CenterPos.Clone();
                    break;
                case YoonVector2N pVector2N:
                    Feature = pVector2N.Clone();
                    Position = pVector2N.Clone();
                    break;
                case YoonVector2D pVector2D:
                    Feature = pVector2D.Clone();
                    Position = pVector2D.Clone();
                    break;
                default:
                    throw new FormatException("[YOONIMAGE EXCEPTION] Object format is not correct");
            }
        }

        public YoonObject(int nLabel, IYoonFigure pFeature, IYoonVector pPosCurrent, double dScore, int nCount)
        {
            Label = nLabel;
            Score = dScore;
            PixelCount = nCount;
            Position = pPosCurrent.Clone();
            Feature = pFeature switch
            {
                IYoonRect pRect => pRect.Clone(),
                IYoonLine pLine => pLine.Clone(),
                IYoonVector pVector => pVector.Clone(),
                _ => throw new FormatException("[YOONIMAGE EXCEPTION] Object format is not correct")
            };
        }

        public int GetLength()
        {
            return 3 + // Label, Score, PixelCount
                   Feature.PropertiesCount + Position.PropertiesCount;
        }

        public bool Set(params string[] pArgs)
        {
            if (pArgs.Length != GetLength()) return false;
            Label = int.Parse(pArgs[0]);
            Score = double.Parse(pArgs[1]);
            PixelCount = int.Parse(pArgs[2]);
            int nCursor = 3;
            Feature.FromArgs(pArgs.Slice(nCursor, nCursor + Feature.PropertiesCount));
            nCursor += Feature.PropertiesCount;
            Position.FromArgs(pArgs.Slice(nCursor, nCursor + Position.PropertiesCount));
            nCursor += Position.PropertiesCount;
            return true;
        }

        public void CopyFrom(IYoonParameter pObject)
        {
            if (pObject is YoonObject pYoonObject)
            {
                Label = pYoonObject.Label;
                Score = pYoonObject.Score;
                PixelCount = pYoonObject.PixelCount;
                Feature = pYoonObject.Feature switch
                {
                    IYoonRect pRect => pRect.Clone(),
                    IYoonLine pLine => pLine.Clone(),
                    IYoonVector pVector => pVector.Clone(),
                    _ => throw new FormatException("[YOONIMAGE EXCEPTION] Object format is not correct")
                };

                Position = pYoonObject.Position.Clone();
            }
        }

        public IYoonParameter Clone()
        {
            return new YoonObject(Label, Feature.Clone(), Position.Clone(), Score, PixelCount);
        }

        public bool Equals(IYoonParameter pObject)
        {
            if (pObject is not YoonObject pYoonObject) return false;
            switch (pYoonObject.Feature)
            {
                case IYoonRect pRect:
                    if (pYoonObject.Label == Label &&
                        pYoonObject.Score == Score &&
                        pRect.Equals(Feature) &&
                        pYoonObject.Position.Equals(Position) &&
                        pYoonObject.PixelCount == PixelCount)
                        return true;
                    break;
                case IYoonLine pLine:
                    if (pYoonObject.Label == Label &&
                        pYoonObject.Score == Score &&
                        pLine.Equals(Feature) &&
                        pYoonObject.Position.Equals(Position) &&
                        pYoonObject.PixelCount == PixelCount)
                        return true;
                    break;
                case IYoonVector pVector:
                    if (pYoonObject.Label == Label &&
                        pYoonObject.Score == Score &&
                        pVector.Equals(Feature) &&
                        pYoonObject.Position.Equals(Position) &&
                        pYoonObject.PixelCount == PixelCount)
                        return true;
                    break;
                default:
                    throw new FormatException("[YOONIMAGE EXCEPTION] Object format is not correct");
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 258276020;
            hashCode = hashCode * -1521134295 + _disposed.GetHashCode();
            hashCode = hashCode * -1521134295 + Label.GetHashCode();
            hashCode = hashCode * -1521134295 + Score.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonFigure>.Default.GetHashCode(Feature);
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector>.Default.GetHashCode(Position);
            hashCode = hashCode * -1521134295 + PixelCount.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is YoonObject @object &&
                   _disposed == @object._disposed &&
                   Label == @object.Label &&
                   Score == @object.Score &&
                   EqualityComparer<IYoonFigure>.Default.Equals(Feature, @object.Feature) &&
                   EqualityComparer<IYoonVector>.Default.Equals(Position, @object.Position) &&
                   PixelCount == @object.PixelCount;
        }

        public bool Equals(YoonObject other)
        {
            return Equals((object) other);
        }

        public static bool operator ==(YoonObject pObjectSource, YoonObject pObjectOther)
        {
            return pObjectSource?.Equals(pObjectOther) == true;
        }

        public static bool operator !=(YoonObject pObjectSource, YoonObject pObjectOther)
        {
            return !(pObjectSource == pObjectOther);
        }
    }
}
