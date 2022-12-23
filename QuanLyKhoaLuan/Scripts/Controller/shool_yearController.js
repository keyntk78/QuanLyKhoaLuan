var shool_yearController = {
    init: function () {
        shool_yearController.registerEvent();
    },
    registerEvent: function () {

        $('#pageSize').change(function () {
            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            shool_yearController.loadData(null, pageSize, keywork);
        });

        $('body').on('click', '#btnSearch', function () {


            var keywork = $('#keywork').val();

            shool_yearController.loadData(null, pageSize, keywork);
        });


        $('body').on('click', '.btnDelete', function (e) {


            var id = $(this).data('id');

            swal({
                title: "Bạn có chắc xóa!",
                text: "Bạn sẽ không thể khôi phục lại được!",
                type: "error",
                confirmButtonText: "Xóa",
                showCancelButton: true,
                cancelButtonText: "Đóng"
            }).then((result) => {
                if (result.value) {
                    shool_yearController.Delete(id)
                }
            });
        });


        $('body').on('click', '.btnDetail', function (e) {
            var id = $(this).data('id');
            shool_yearController.Detail(id);
        });

        pageSize = $('#pageSize').val();
        shool_yearController.loadData(null, pageSize, "");
    },
    loadData: function (page, pageSize, keywork) {
        $.ajax({
            url: '/Admin/School_year/GetShoolYearData',
            type: 'GET',
  /*          data: { page: page, pageSize: pageSize, keywork: keywork },*/
            data: { page: page, pageSize: pageSize, keywork: keywork },
            success: function (res) {

                if (res.TotalItem >= 0) {
                    var item = res.Data;
                    var html = "";
                    for (let i = 0; i < res.Data.length; i++) {
                        var date_start = shool_yearController.getDateIfDate(item[i].start_date);
                        var date_end = shool_yearController.getDateIfDate(item[i].end_date);


                        html += "<tr>";
                        html += ` <td>${i + 1}</td>`;
                        html += ` <td>${item[i].name}</td>`;
                        html += ` <td>${date_start}</td>`;
                        html += ` <td>${date_end}</td>`;

                        html += `<td>`;
                        html += `<button data-id="${item[i].school_year_id}" class="btn btn-danger btnDelete" title="Xóa" ><i class="fa-solid fa-trash"></i></button> | `
                        html += `<a  href="/Admin/School_year/Edit/${item[i].school_year_id}"  class="btn btn-success btnEdit" title="Cập nhật"><i class="fa-solid fa-pen-to-square"></i></a> | 
                                <button data-id="${item[i].school_year_id}" class="btn btn-info btnDetail" data-toggle="modal" data-target="#exampleModal" title="Xem chi tiết" ><i class="fa-solid fa-eye"></i></button>
                                </td >`
                        html += `</tr>`;
                    }
                }
                $('#show_data').html(html);
                shool_yearController.Pagination(res.CurrentPage, res.NumberPage, res.PageSize);
            }
        })
    },

    getDateIfDate: function (d) {
        var m = d.match(/\/Date\((\d+)\)\//);
        return m ? (new Date(+m[1])).toLocaleDateString('es-SV', { month: '2-digit', day: '2-digit', year: 'numeric' }) : d;
    },

    Pagination: function (currentPage, numberpage, pageSize) {
        if (numberpage > 0) {
            var str = `<nav aria-label="Page navigation example"><ul class="pagination">`;
            if (currentPage != 1) {
                str += ` <li class="page-item"><a class="page-link" onclick="shool_yearController.NextPage(${currentPage - 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-left"></i></a></li>`;
            }
            for (let i = 1; i <= numberpage; i++) {
                if (currentPage === i) {
                    str += `<li class="page-item active"><a class="page-link" href="javascript:void(0);">${i}</a></li>`;
                } else {

                    str += `<li class="page-item"><a class="page-link" onclick="shool_yearController.NextPage(${i}, ${pageSize})" href="javascript:void(0);">${i}</a></li>`;
                }
            }

            if (currentPage != numberpage) {
                str += ` <li class="page-item"><a class="page-link" onclick="shool_yearController.NextPage(${currentPage + 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-right"></i></a></li>`;
            }
            str += `</ul></nav>`;
            $('#pagination').html(str);
        }
    },

    NextPage: function (page, pageSize) {
        var keywork = $('#keywork').val();
        shool_yearController.loadData(page, pageSize, keywork);

    },


    Delete: function (id) {
        $.ajax({
            url: "/Admin/School_year/Delete",
            type: "POST",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    pageSize = $('#pageSize').val();
                    shool_yearController.loadData(null, pageSize, "");
                    Swal.fire(
                        'Đã Xóa!',
                        ' Niên khóa đã xóa.',
                        'success'
                    )
                } else {
                    Swal.fire(
                        'Xóa thất bại!',
                        'Thất bại.',
                        'error'
                    )
                }
            }
        });
    },

    Detail: function (id) {
        $.ajax({
            url: "/Admin/School_year/Detail",
            type: "GET",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    $('#name').text(res.Data.name);
                    $('#start-date').text(shool_yearController.getDateIfDate(res.Data.start_date));
                    $('#end-date').text(shool_yearController.getDateIfDate(res.Data.end_date));
                } else {
                    alert("Có lỗi");
                }
            }
        });
    },
};

shool_yearController.init();