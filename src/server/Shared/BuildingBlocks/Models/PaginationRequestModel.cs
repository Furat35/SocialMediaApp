﻿namespace BuildingBlocks.Models
{
    public class PaginationRequestModel
    {
        private int _page = 1;
        private int _pageSize = 10;
        public int Page { get => _page; set => _page = value < 1 ? 1 : value; }
        public int PageSize { get => _pageSize; set => _pageSize = value < 1 ? 10 : value; }
    }
}
