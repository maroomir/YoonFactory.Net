using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoonFactory.Image
{
    public class YoonDataset : IDisposable, IList<YoonObject>
    {
        #region IDisposable Support

        ~YoonDataset()
        {
            this.Dispose(false);
        }

        private bool _disposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed) return;
            if (disposing)
            {
                Clear();
            }

            _pListObject = null;
            _disposed = true;
        }

        #endregion

        protected List<YoonObject> _pListObject = new List<YoonObject>();

        public List<int> Labels => _pListObject.Select(pObject => pObject.Label).ToList();

        public List<YoonImage> Images => _pListObject.Select(pObject => pObject.ObjectImage).ToList();

        public List<IYoonFigure> Features => _pListObject.Select(pObject => pObject.Feature).ToList();

        public List<int> PixelCounts => _pListObject.Select(pObject => pObject.PixelCount).ToList();

        public List<double> Scores => _pListObject.Select(pObject => pObject.Score).ToList();

        public List<IYoonVector> References => _pListObject.Select(pObject => pObject.ReferencePosition).ToList();

        public YoonObject this[int index]
        {
            get
            {
                if (_pListObject == null)
                    throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
                if (index < 0 || index >= _pListObject.Count)
                    throw new IndexOutOfRangeException("[YOONIMAGE EXCEPTION] Index must be within the bounds");
                return _pListObject[index];
            }
            set
            {
                if (_pListObject == null)
                    throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
                if (index < 0 || index >= _pListObject.Count)
                    throw new IndexOutOfRangeException("[YOONIMAGE EXCEPTION] Index must be within the bounds");
                _pListObject[index] = value;
            }
        }

        public int Count => _pListObject.Count;

        public bool IsReadOnly => ((ICollection<YoonObject>) _pListObject).IsReadOnly;

        public void Add(YoonObject item)
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            _pListObject.Add(item);
        }

        public void Clear()
        {
            if (_pListObject != null)
                _pListObject.Clear();
        }

        public bool Contains(YoonObject item)
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            return _pListObject.Contains(item);
        }

        public void CopyTo(YoonObject[] array, int arrayIndex)
        {
            if (_pListObject == null || array == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw new IndexOutOfRangeException("[YOONIMAGE EXCEPTION] Index must be within the bounds");
            _pListObject.CopyTo(array, arrayIndex);
        }

        public YoonDataset Clone()
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            YoonDataset pDataset = new YoonDataset();
            pDataset._pListObject = new List<YoonObject>(_pListObject);
            return pDataset;
        }

        public void CopyFrom(YoonDataset pList)
        {
            _pListObject = new List<YoonObject>(pList._pListObject);
        }

        public List<int> CopyLabels()
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            return new List<int>(Labels);
        }

        public List<YoonImage> CopyImages()
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            return new List<YoonImage>(Images);
        }

        public List<double> CopyScores()
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            return new List<double>(Scores);
        }

        public List<int> CopyPixelCounts()
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            return new List<int>(PixelCounts);
        }

        public List<IYoonFigure> CopyFeatures()
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            return new List<IYoonFigure>(Features);
        }

        public List<IYoonVector> CopyReferences()
        {

            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            return new List<IYoonVector>(References);
        }

        public IEnumerator<YoonObject> GetEnumerator()
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            return _pListObject.GetEnumerator();
        }

        public int IndexOf(YoonObject item)
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            return _pListObject.IndexOf(item);
        }

        public int LabelIndexOf(int nLabel)
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            return Labels.IndexOf(nLabel);
        }

        public int ImageIndexOf(YoonImage pImage)
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            return Images.IndexOf(pImage);
        }

        public YoonObject Search(int nLabel)
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            YoonObject pObject = null;
            foreach (YoonObject pItem in _pListObject)
            {
                if (pItem.Label != nLabel) continue;
                pObject = pItem.Clone() as YoonObject;
                break;
            }

            if (pObject == null)
                throw new NullReferenceException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            return pObject;
        }

        public void Swap(int index1, int index2)
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            if (index1 < 0 || index1 >= _pListObject.Count ||
                index2 < 0 || index2 >= _pListObject.Count)
                throw new IndexOutOfRangeException("[YOONIMAGE EXCEPTION] Index must be within the bounds");
            YoonObject pObject1 = _pListObject[index1].Clone() as YoonObject;
            YoonObject pObject2 = _pListObject[index2].Clone() as YoonObject;
            _pListObject[index2] = pObject1;
            _pListObject[index1] = pObject2;
        }

        public void Insert(int index, YoonObject item)
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            if (index < 0 || index >= _pListObject.Count)
                throw new IndexOutOfRangeException("[YOONIMAGE EXCEPTION] Index must be within the bounds");
            _pListObject.Insert(index, item);
        }

        public bool Remove(YoonObject item)
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            return _pListObject.Remove(item);
        }

        public void RemoveAt(int index)
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            if (index < 0 || index >= _pListObject.Count)
                throw new IndexOutOfRangeException("[YOONIMAGE EXCEPTION] Index must be within the bounds");
            RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _pListObject.GetEnumerator();
        }

        public void SortLabels(eYoonDir2DMode nMode)
        {
            if (_pListObject == null)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            int iSearch;
            switch (nMode)
            {
                case eYoonDir2DMode.Increase:
                    for (int i = 0; i < _pListObject.Count - 1; i++)
                    {
                        iSearch = i;
                        for (int j = i + 1; j < _pListObject.Count; j++)
                        {
                            int nMinLabel = _pListObject[iSearch].Label;
                            if (_pListObject[j].Label < nMinLabel)
                                iSearch = j;
                        }

                        if (iSearch == i) continue;
                        Swap(i, iSearch);
                    }

                    break;
                case eYoonDir2DMode.Decrease:
                    for (int i = 0; i < _pListObject.Count - 1; i++)
                    {
                        iSearch = i;
                        for (int j = i + 1; j < _pListObject.Count; j++)
                        {
                            int nMaxLabel = _pListObject[iSearch].Label;
                            if (_pListObject[j].Label > nMaxLabel)
                                iSearch = j;
                        }

                        if (iSearch == i) continue;
                        Swap(i, iSearch);
                    }

                    break;
                default:
                    throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Sorting mode is not correct");
            }
        }

        public void SortFeatures(eYoonDir2D nDir)
        {
            if (_pListObject == null || _pListObject.Count == 0)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            if (_pListObject[0].Feature is YoonRect2N)
                SortRect2N(nDir);
            if (_pListObject[0].Feature is YoonRect2D)
                SortRect2D(nDir);
            if (_pListObject[0].Feature is YoonRectAffine2D)
                SortRectAffine2D(nDir);
            else
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] This type is not supported");
        }

        private void SortRect2N(eYoonDir2D nDir)
        {
            for (int i = 0; i < _pListObject.Count - 1; i++)
            {
                int iSearch = i;
                for (int j = i + 1; j < _pListObject.Count; j++)
                {
                    YoonRect2N pRectMin = _pListObject[iSearch].Feature as YoonRect2N;
                    YoonRect2N pRectCurr = _pListObject[j].Feature as YoonRect2N;
                    Debug.Assert(pRectMin != null, nameof(pRectMin) + " != null");
                    Debug.Assert(pRectCurr != null, nameof(pRectCurr) + " != null");
                    int nDiff = 0;
                    int nHeight = 0;
                    // Height difference takes precedence then the width difference
                    switch (nDir)
                    {
                        case eYoonDir2D.TopLeft:
                            nDiff = Math.Abs(pRectMin.Top - pRectCurr.Top);
                            nHeight = pRectMin.Bottom - pRectMin.Top;
                            if (nDiff <= nHeight / 2)
                            {
                                if (pRectCurr.Left < pRectMin.Left)
                                    iSearch = j;
                            }
                            else
                            {
                                if (pRectCurr.Top < pRectMin.Top)
                                    iSearch = j;
                            }

                            break;
                        case eYoonDir2D.TopRight:
                            nDiff = Math.Abs(pRectMin.Top - pRectCurr.Top);
                            nHeight = pRectMin.Bottom - pRectMin.Top;
                            if (nDiff <= nHeight / 2)
                            {
                                if (pRectCurr.Right > pRectMin.Right)
                                    iSearch = j;
                            }
                            else
                            {
                                if (pRectCurr.Top < pRectMin.Top)
                                    iSearch = j;
                            }

                            break;
                        case eYoonDir2D.Left:
                            if (pRectCurr.Left < pRectMin.Left)
                                iSearch = j;
                            break;
                        case eYoonDir2D.Right:
                            if (pRectCurr.Right > pRectMin.Right)
                                iSearch = j;
                            break;
                        default: // Top-Left default
                            nDiff = Math.Abs(pRectMin.Top - pRectCurr.Top);
                            nHeight = pRectMin.Bottom - pRectMin.Top;
                            if (nDiff <= nHeight / 2)
                                continue;
                            if (pRectCurr.Left < pRectMin.Left)
                                iSearch = j;
                            break;
                    }
                }

                if (iSearch == i) continue;
                Swap(i, iSearch);
            }
        }

        private void SortRect2D(eYoonDir2D nDir)
        {
            for (int i = 0; i < _pListObject.Count - 1; i++)
            {
                int iSearch = i;
                for (int j = i + 1; j < _pListObject.Count; j++)
                {
                    YoonRect2D pRectMin = _pListObject[iSearch].Feature as YoonRect2D;
                    YoonRect2D pRectCurr = _pListObject[j].Feature as YoonRect2D;
                    Debug.Assert(pRectMin != null, nameof(pRectMin) + " != null");
                    Debug.Assert(pRectCurr != null, nameof(pRectCurr) + " != null");
                    double dHeight = 0;
                    double dDiff = 0;
                    switch (nDir)
                    {
                        case eYoonDir2D.TopLeft:
                            dDiff = Math.Abs(pRectMin.Top - pRectCurr.Top);
                            dHeight = pRectMin.Bottom - pRectMin.Top;
                            if (dDiff <= dHeight / 2)
                            {
                                if (pRectCurr.Left < pRectMin.Left)
                                    iSearch = j;
                            }
                            else
                            {
                                if (pRectCurr.Top < pRectMin.Top)
                                    iSearch = j;
                            }

                            break;
                        case eYoonDir2D.TopRight:
                            dDiff = Math.Abs(pRectMin.Top - pRectCurr.Top);
                            dHeight = pRectMin.Bottom - pRectMin.Top;
                            if (dDiff <= dHeight / 2)
                            {
                                if (pRectCurr.Right > pRectMin.Right)
                                    iSearch = j;
                            }
                            else
                            {
                                if (pRectCurr.Top < pRectMin.Top)
                                    iSearch = j;
                            }

                            break;
                        case eYoonDir2D.Left:
                            if (pRectCurr.Left < pRectMin.Left)
                                iSearch = j;
                            break;
                        case eYoonDir2D.Right:
                            if (pRectCurr.Right > pRectMin.Right)
                                iSearch = j;
                            break;
                        default:
                            dDiff = Math.Abs(pRectMin.Top - pRectCurr.Top);
                            dHeight = pRectMin.Bottom - pRectMin.Top;
                            if (dDiff <= dHeight / 2)
                                continue;
                            if (pRectCurr.Left < pRectMin.Left)
                                iSearch = j;
                            break;
                    }
                }

                if (iSearch == i) continue;
                Swap(i, iSearch);
            }
        }

        private void SortRectAffine2D(eYoonDir2D nDir)
        {
            for (int i = 0; i < _pListObject.Count - 1; i++)
            {
                int iSearch = i;
                for (int j = i + 1; j < _pListObject.Count; j++)
                {
                    YoonRectAffine2D pRectMin = _pListObject[iSearch].Feature as YoonRectAffine2D;
                    YoonRectAffine2D pRectCurr = _pListObject[j].Feature as YoonRectAffine2D;
                    Debug.Assert(pRectMin != null, nameof(pRectMin) + " != null");
                    Debug.Assert(pRectCurr != null, nameof(pRectCurr) + " != null");
                    double dDiff = 0;
                    double dHeight = 0;
                    switch (nDir)
                    {
                        case eYoonDir2D.TopLeft:
                            dDiff = Math.Abs(pRectMin.Top - pRectCurr.Top);
                            dHeight = pRectMin.Bottom - pRectMin.Top;
                            if (dDiff <= dHeight / 2)
                            {
                                if (pRectCurr.Left < pRectMin.Left)
                                    iSearch = j;
                            }
                            else
                            {
                                if (pRectCurr.Top < pRectMin.Top)
                                    iSearch = j;
                            }

                            break;
                        case eYoonDir2D.TopRight:
                            dDiff = Math.Abs(pRectMin.Top - pRectCurr.Top);
                            dHeight = pRectMin.Bottom - pRectMin.Top;
                            if (dDiff <= dHeight / 2)
                            {
                                if (pRectCurr.Right > pRectMin.Right)
                                    iSearch = j;
                            }
                            else
                            {
                                if (pRectCurr.Top < pRectMin.Top)
                                    iSearch = j;
                            }

                            break;
                        case eYoonDir2D.Left:
                            if (pRectCurr.Left < pRectMin.Left)
                                iSearch = j;
                            break;
                        case eYoonDir2D.Right:
                            if (pRectCurr.Right > pRectMin.Right)
                                iSearch = j;
                            break;
                        default:
                            dDiff = Math.Abs(pRectMin.Top - pRectCurr.Top);
                            dHeight = pRectMin.Bottom - pRectMin.Top;
                            if (dDiff <= dHeight / 2)
                                continue;
                            if (pRectCurr.Left < pRectMin.Left)
                                iSearch = j;
                            break;
                    }
                }

                if (iSearch == i) continue;
                Swap(i, iSearch);
            }
        }

        public void CombineFeatures()
        {
            if (_pListObject == null || _pListObject.Count == 0)
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] Objects was not ordered");
            if (_pListObject[0].Feature is YoonRect2N)
                CombineRect2N();
            if (_pListObject[0].Feature is YoonRect2D)
                CombineRect2D();
            if (_pListObject[0].Feature is YoonRectAffine2D)
                CombineRectAffine2D();
            else
                throw new InvalidOperationException("[YOONIMAGE EXCEPTION] This type is not supported");
        }

        private void CombineRect2N()
        {
            bool bCombine = false;
            // Copy the original list, then clear that
            List<YoonObject> pTempSet = new List<YoonObject>();
            foreach (YoonObject pObject in _pListObject)
            {
                pTempSet.Add(pObject.Clone() as YoonObject);
            }

            _pListObject.Clear();
            // Find all of squares to overlap
            for (int i = 0; i < pTempSet.Count; i++)
            {
                YoonRect2N pRect1 = pTempSet[i].Feature as YoonRect2N;
                Debug.Assert(pRect1 != null, nameof(pRect1) + " != null");
                YoonRect2N combineRect = new YoonRect2N(0, 0, 0, 0);
                if (pRect1.Width == 0)
                    continue;
                bCombine = false;
                for (int j = 0; j < pTempSet.Count; j++)
                {
                    if (i == j) continue;
                    YoonRect2N pRect2 = pTempSet[j].Feature as YoonRect2N;
                    Debug.Assert(pRect2 != null, nameof(pRect2) + " != null");
                    if (pRect2.Width == 0)
                        continue;
                    // Overlap two rectangles
                    if ((pRect1.Left > pRect2.Left) && (pRect1.Left < pRect2.Right))
                    {
                        if ((pRect1.Top >= pRect2.Top) && (pRect1.Top <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Bottom >= pRect2.Top) && (pRect1.Bottom <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Top <= pRect2.Top) && (pRect1.Bottom >= pRect2.Bottom))
                            bCombine = true;
                    }

                    if (pRect1.Right > pRect2.Left && pRect1.Right < pRect2.Right)
                    {
                        if ((pRect1.Top >= pRect2.Top) && (pRect1.Top <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Bottom >= pRect2.Top) && (pRect1.Bottom <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Top <= pRect2.Top) && (pRect1.Bottom >= pRect2.Bottom))
                            bCombine = true;
                    }

                    if ((pRect1.Left <= pRect2.Left) && (pRect1.Right >= pRect2.Right))
                    {
                        if ((pRect1.Top >= pRect2.Top) && (pRect1.Top <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Bottom >= pRect2.Top) && (pRect1.Bottom <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Top <= pRect2.Top) && (pRect1.Bottom >= pRect2.Bottom))
                            bCombine = true;
                    }

                    if (bCombine)
                    {
                        combineRect.CenterPos.X = (pRect1.Left < pRect2.Left) ? pRect1.CenterPos.X : pRect2.CenterPos.X;
                        combineRect.Width = (pRect1.Right > pRect2.Right)
                            ? pRect1.Right - combineRect.Left
                            : pRect2.Right - combineRect.Left;
                        combineRect.CenterPos.Y = (pRect1.Top < pRect2.Top) ? pRect1.CenterPos.Y : pRect2.CenterPos.Y;
                        combineRect.Height = (pRect1.Bottom > pRect2.Bottom)
                            ? pRect1.Bottom - combineRect.Top
                            : pRect2.Bottom - combineRect.Top;
                        pTempSet[i].Feature = new YoonRect2N(0, 0, 0, 0);
                        pTempSet[j].Feature = combineRect;
                        break;
                    }
                }
            }

            // Sort only valid squares
            for (int i = 0; i < pTempSet.Count; i++)
            {
                IYoonFigure pObject = pTempSet[i].Feature.Clone();
                if (pObject is YoonRect2N pRect)
                {
                    if (pRect.Right != 0)
                    {
                        _pListObject.Add(new YoonObject(pTempSet[i].Label, pObject,
                            (YoonImage) pTempSet[i].ObjectImage.Clone(), pTempSet[i].Score, pTempSet[i].PixelCount));
                    }
                }
                else
                    break;
            }

            pTempSet.Clear();
        }

        private void CombineRect2D()
        {
            bool bCombine = false;
            List<YoonObject> pListTemp = new List<YoonObject>();
            foreach (YoonObject pObject in _pListObject)
            {
                pListTemp.Add(pObject.Clone() as YoonObject);
            }

            _pListObject.Clear();
            for (int i = 0; i < pListTemp.Count; i++)
            {
                YoonRect2D pRect1 = pListTemp[i].Feature as YoonRect2D;
                Debug.Assert(pRect1 != null, nameof(pRect1) + " != null");
                YoonRect2D combineRect = new YoonRect2D(0, 0, 0, 0);
                if (pRect1.Width == 0)
                    continue;
                bCombine = false;
                for (int j = 0; j < pListTemp.Count; j++)
                {
                    if (i == j) continue;
                    YoonRect2D pRect2 = pListTemp[j].Feature as YoonRect2D;
                    Debug.Assert(pRect2 != null, nameof(pRect2) + " != null");
                    if (pRect2.Width == 0)
                        continue;
                    if ((pRect1.Left > pRect2.Left) && (pRect1.Left < pRect2.Right))
                    {
                        if ((pRect1.Top >= pRect2.Top) && (pRect1.Top <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Bottom >= pRect2.Top) && (pRect1.Bottom <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Top <= pRect2.Top) && (pRect1.Bottom >= pRect2.Bottom))
                            bCombine = true;
                    }

                    if (pRect1.Right > pRect2.Left && pRect1.Right < pRect2.Right)
                    {
                        if ((pRect1.Top >= pRect2.Top) && (pRect1.Top <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Bottom >= pRect2.Top) && (pRect1.Bottom <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Top <= pRect2.Top) && (pRect1.Bottom >= pRect2.Bottom))
                            bCombine = true;
                    }

                    if ((pRect1.Left <= pRect2.Left) && (pRect1.Right >= pRect2.Right))
                    {
                        if ((pRect1.Top >= pRect2.Top) && (pRect1.Top <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Bottom >= pRect2.Top) && (pRect1.Bottom <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Top <= pRect2.Top) && (pRect1.Bottom >= pRect2.Bottom))
                            bCombine = true;
                    }

                    if (bCombine)
                    {
                        combineRect.CenterPos.X = (pRect1.Left < pRect2.Left) ? pRect1.CenterPos.X : pRect2.CenterPos.X;
                        combineRect.Width = (pRect1.Right > pRect2.Right)
                            ? pRect1.Right - combineRect.Left
                            : pRect2.Right - combineRect.Left;
                        combineRect.CenterPos.Y = (pRect1.Top < pRect2.Top) ? pRect1.CenterPos.Y : pRect2.CenterPos.Y;
                        combineRect.Height = (pRect1.Bottom > pRect2.Bottom)
                            ? pRect1.Bottom - combineRect.Top
                            : pRect2.Bottom - combineRect.Top;
                        pListTemp[i].Feature = new YoonRect2D(0, 0, 0, 0);
                        pListTemp[j].Feature = combineRect;
                        break;
                    }
                }
            }

            for (int i = 0; i < pListTemp.Count; i++)
            {
                IYoonFigure pObject = pListTemp[i].Feature.Clone();
                if (pObject is YoonRect2D pRect)
                {
                    if (pRect.Right != 0)
                    {
                        _pListObject.Add(new YoonObject(pListTemp[i].Label, pObject,
                            (YoonImage) pListTemp[i].ObjectImage.Clone(), pListTemp[i].Score, pListTemp[i].PixelCount));
                    }
                }
                else
                    break;
            }

            pListTemp.Clear();
        }

        private void CombineRectAffine2D()
        {
            bool bCombine = false;
            List<YoonObject> pListTemp = new List<YoonObject>();
            foreach (YoonObject pObject in _pListObject)
            {
                pListTemp.Add(pObject.Clone() as YoonObject);
            }

            _pListObject.Clear();
            for (int i = 0; i < pListTemp.Count; i++)
            {
                YoonRectAffine2D pRect1 = pListTemp[i].Feature as YoonRectAffine2D;
                Debug.Assert(pRect1 != null, nameof(pRect1) + " != null");
                YoonRectAffine2D combineRect = new YoonRectAffine2D(0, 0, 0);
                if (pRect1.Width == 0)
                    continue;
                bCombine = false;
                for (int j = 0; j < pListTemp.Count; j++)
                {
                    if (i == j) continue;
                    YoonRectAffine2D pRect2 = pListTemp[j].Feature as YoonRectAffine2D;
                    Debug.Assert(pRect2 != null, nameof(pRect2) + " != null");
                    if (pRect2.Width == 0)
                        continue;
                    if ((pRect1.Left > pRect2.Left) && (pRect1.Left < pRect2.Right))
                    {
                        if ((pRect1.Top >= pRect2.Top) && (pRect1.Top <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Bottom >= pRect2.Top) && (pRect1.Bottom <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Top <= pRect2.Top) && (pRect1.Bottom >= pRect2.Bottom))
                            bCombine = true;
                    }

                    if (pRect1.Right > pRect2.Left && pRect1.Right < pRect2.Right)
                    {
                        if ((pRect1.Top >= pRect2.Top) && (pRect1.Top <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Bottom >= pRect2.Top) && (pRect1.Bottom <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Top <= pRect2.Top) && (pRect1.Bottom >= pRect2.Bottom))
                            bCombine = true;
                    }

                    if ((pRect1.Left <= pRect2.Left) && (pRect1.Right >= pRect2.Right))
                    {
                        if ((pRect1.Top >= pRect2.Top) && (pRect1.Top <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Bottom >= pRect2.Top) && (pRect1.Bottom <= pRect2.Bottom))
                            bCombine = true;
                        if ((pRect1.Top <= pRect2.Top) && (pRect1.Bottom >= pRect2.Bottom))
                            bCombine = true;
                    }

                    if (bCombine)
                    {
                        combineRect.CenterPos.X = (pRect1.Left < pRect2.Left) ? pRect1.CenterPos.X : pRect2.CenterPos.X;
                        combineRect.Width = (pRect1.Right > pRect2.Right)
                            ? pRect1.Right - combineRect.Left
                            : pRect2.Right - combineRect.Left;
                        combineRect.CenterPos.Y = (pRect1.Top < pRect2.Top) ? pRect1.CenterPos.Y : pRect2.CenterPos.Y;
                        combineRect.Height = (pRect1.Bottom > pRect2.Bottom)
                            ? pRect1.Bottom - combineRect.Top
                            : pRect2.Bottom - combineRect.Top;
                        pListTemp[i].Feature = new YoonRectAffine2D(0, 0, 0);
                        pListTemp[j].Feature = combineRect;
                        break;
                    }
                }
            }

            for (int i = 0; i < pListTemp.Count; i++)
            {
                IYoonFigure pObject = pListTemp[i].Feature.Clone();
                if (pObject is YoonRectAffine2D pRect)
                {
                    if (pRect.Right != 0)
                    {
                        _pListObject.Add(new YoonObject(pListTemp[i].Label, pObject,
                            (YoonImage) pListTemp[i].ObjectImage.Clone(), pListTemp[i].Score, pListTemp[i].PixelCount));
                    }
                }
                else
                    break;
            }

            pListTemp.Clear();
        }
    }
}