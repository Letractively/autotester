using System;
using System.Collections.Generic;
using System.Text;
using mshtml;

using Shrinerain.AutoTester.Function;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestImage : HTMLGuiTestObject
    {

        #region fields

        protected string _src;

        protected IHTMLImgElement _imgElement;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public HTMLTestImage(IHTMLElement element)
            : base(element)
        {
            try
            {
                _imgElement = (IHTMLImgElement)element;
            }
            catch
            {
                throw new CanNotBuildObjectException("Can not convert to IHTMLImgElement.");
            }
        }

        #endregion

        #region public methods

        #endregion

        #region private methods


        #endregion

        #endregion

    }
}
