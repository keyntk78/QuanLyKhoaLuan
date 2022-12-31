var theseController = {
    init: function () {
        theseController.registerEvent();
    },
    registerEvent: function () {
        $('#pageSize').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var status = $('#status').val();
            var major_id = $('#select_major_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            theseController.loadData(null, pageSize, keywork, status, major_id, shool_year_id);


        });


        $('body').on('click', '#btnSearch', function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var status = $('#status').val();
            var major_id = $('#select_major_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            theseController.loadData(null, pageSize, keywork, status, major_id, shool_year_id);

        });

        $('#status').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var status = $('#status').val();
            var major_id = $('#select_major_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            theseController.loadData(null, pageSize, keywork, status, major_id, shool_year_id);

        })


        $('#select_major_id').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var status = $('#status').val();
            var major_id = $('#select_major_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            theseController.loadData(null, pageSize, keywork, status, major_id, shool_year_id);

        });

        $('#select_shoolyear_id').change(function () {


            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var status = $('#status').val();
            var major_id = $('#select_major_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            theseController.loadData(null, pageSize, keywork, status, major_id, shool_year_id);

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
                    theseController.Delete(id)
                }
            });
        });

        $('body').on('click', '.btnDetail', function (e) {
            var id = $(this).data('id');
            theseController.Detail(id);
        });
        var pageSize = $('#pageSize').val();
        theseController.loadData(null, pageSize, "", "", "", "");
    },
    loadData: function (page, pageSize, keywork, status, major_id, shool_year_id) {

        $.ajax({
            url: '/Admin/Theses/GetThesesData',
            type: 'GET',

            data: { page: page, pageSize: pageSize, keywork: keywork, status: status, major_id: major_id, shool_year_id: shool_year_id },

            success: function (res) {
                if (res.TotalItem >= 0) {

                    var item = res.Data;
                    var html = "";
                    for (let i = 0; i < res.Data.length; i++) {
                        var file_thesis = `<span class="badge badge-info">Đã nộp</span>`;
                        if (item[i].theses.file_thesis == null) {
                            file_thesis = `<span class="badge badge-warning">Chưa nộp</span>`;
                        }

                        var total_score = `<span class="badge badge-warning">Chưa có</span>`;
                        if (item[i].theses.total_score != null) {
                            total_score = `<span class="badge badge-info">${item[i].theses.total_score}</span>`;
                        }

                        var result = `<span class="badge badge-warning">Chưa có</span>`;
                        if (item[i].theses.result != null) {
                            result = `<span class="badge badge-info">${item[i].theses.result}</span>`;
                        }

                        html += "<tr>";
                        html += ` <td>${item[i].theses.code}</td>`;
                        html += ` <td>${item[i].topic.name}</td>`;
                        html += ` </td>`;
                        html += `<td>`;
                        html += `<p>${item[i].major.name}</p>`;
                        html += ` </td>`;
                        html += ` <td>${item[i].school_year.name}</td>`;
                        html += ` <td>${theseController.getDateIfDate(item[i].theses.start_date)} đến ${theseController.getDateIfDate(item[i].theses.end_date)} </td>`;
                        html += ` <td>${file_thesis}</td>`;
                        html += ` <td>${total_score}</td>`;
                        html += ` <td>${result}</td>`;

                        html += `<td>`;
                        html += `<button data-id="${item[i].theses.thesis_id}" class="btn btn-danger btnDelete" title="Xóa" ><i class="fa-solid fa-trash"></i></button> | `
                        html += `<a  href="/Admin/Theses/Edit/${item[i].theses.thesis_id}"  class="btn btn-success btnEdit" title="Cập nhật"><i class="fa-solid fa-pen-to-square"></i></a>
                                    | <button data-id="${item[i].theses.thesis_id}" class="btn btn-info btnDetail mt-2" data-toggle="modal" data-target="#exampleModal" title="Xem chi tiết" ><i class="fa-solid fa-eye"></i></button>
                                    </td >`
                        html += `</tr>`;
                    }
                }
                $('#show_data').html(html);
                theseController.Pagination(res.CurrentPage, res.NumberPage, res.PageSize);
            },
        })
    },
    Pagination: function (currentPage, numberpage, pageSize) {
        if (numberpage > 0) {
            var str = `<nav aria-label="Page navigation example"><ul class="pagination">`;
            if (currentPage != 1) {
                str += ` <li class="page-item"><a class="page-link" onclick="theseController.NextPage(${currentPage - 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-left"></i></a></li>`;
            }
            for (let i = 1; i <= numberpage; i++) {
                if (currentPage === i) {
                    str += `<li class="page-item active"><a class="page-link" href="javascript:void(0);">${i}</a></li>`;
                } else {

                    str += `<li class="page-item"><a class="page-link" onclick="theseController.NextPage(${i}, ${pageSize})" href="javascript:void(0);">${i}</a></li>`;
                }
            }

            if (currentPage != numberpage) {
                str += ` <li class="page-item"><a class="page-link" onclick="theseController.NextPage(${currentPage + 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-right"></i></a></li>`;
            }
            str += `</ul></nav>`;
            $('#pagination').html(str);
        }
    },

    NextPage: function (page, pageSize) {


        var keywork = $('#keywork').val();
        pageSize = $('#pageSize').val();
        var status = $('#status').val();
        var major_id = $('#select_major_id').val();
        var shool_year_id = $('#select_shoolyear_id').val();

        theseController.loadData(page, pageSize, keywork, status, major_id, shool_year_id);

    },
    getDateIfDate: function (d) {
        var m = d.match(/\/Date\((\d+)\)\//);
        return m ? (new Date(+m[1])).toLocaleDateString('es-SV', { month: '2-digit', day: '2-digit', year: 'numeric' }) : d;
    },

    Delete: function (id) {
        $.ajax({
            url: "/Admin/Theses/Delete",
            type: "POST",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    pageSize = $('#pageSize').val();
                    theseController.loadData(null, pageSize, "", "", "", "");
                    Swal.fire(
                        'Đã Xóa!',
                        'Khóa luận đã xóa.',
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
            url: "/Admin/Theses/Detail",
            type: "GET",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    var date = theseController.getDateIfDate(res.Data.theses.start_date) + " đến " + theseController.getDateIfDate(res.Data.theses.end_date);
                    var date_outline = theseController.getDateIfDate(res.Data.theses.start_date_outline) + " đến " + theseController.getDateIfDate(res.Data.theses.end_date_outline);
                    var date_thesis = theseController.getDateIfDate(res.Data.theses.start_date_thesis) + " đến " + theseController.getDateIfDate(res.Data.theses.end_date_thesis);

                    $('#code').text(res.Data.theses.code);
                    $('#topic_name').text(res.Data.topic.name);
                    $('#date').text(date);
                    $('#date_outline').text(date_outline);
                    $('#date_thesis').text(date_thesis);
                    $('#school_year').text(res.Data.school_year.name);
                    $('#major').text(res.Data.major.name);
                    $('#council').text(res.Data.council.name);
                    $('#lecturer').text(res.Data.lecturer.full_name);

                } else {
                    alert("Có lỗi");
                }
            }
        });
    },

};

theseController.init();