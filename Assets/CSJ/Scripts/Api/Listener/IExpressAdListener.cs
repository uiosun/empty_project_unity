﻿//------------------------------------------------------------------------------
// Copyright (c) 2018-2019 Beijing Bytedance Technology Co., Ltd.
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//------------------------------------------------------------------------------

namespace ByteDance.Union
{
    using System.Collections.Generic;
	/// <summary>
	/// The listener for express Ad.
	/// </summary>
	public interface IExpressAdListener
	{
		/// <summary>
		/// Invoke when load Ad error.
		/// </summary>
		void OnError(int code, string message);

		/// <summary>
		/// Invoke when the Ad load success.
		/// </summary>
        void OnExpressAdLoad(List<ExpressAd> ads);
    }
}
