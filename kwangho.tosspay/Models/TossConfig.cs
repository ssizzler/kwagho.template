using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kwangho.tosspay.Models
{
    /// <summary>
    /// Toss 상점 설정 정보
    /// </summary>
    public class TossConfig
    {
        /// <summary>
        /// 상점 ID, 토스에 등록된 상점 ID
        /// </summary>
        public string ClientId { get; set; } = null!;

        /// <summary>
        /// 상점 Client Key, 토스 개발자페이지에서 확인
        /// </summary>
        public string ClientKey { get; set; } = null!;

        /// <summary>
        /// 상점 비번: 토스 개발자 페이지에서 확인
        /// </summary>
        public string SecretKey { get; set; } = null!;

        /// <summary>
        /// 결제사이트 URL
        /// </summary>
        public Uri PaymentUrl { get; set; } = null!;
    }
}
