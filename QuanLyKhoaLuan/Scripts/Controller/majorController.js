var majorController = {
    Init: function () {
        majorController.registerEvent();
    },
    registerEvent: function () {
        $('#pageSize').change(function () {
            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            var department_id = $('#select_department_id').val();
            if (department_id == null) {
                majorController.loadData(null, pageSize, keywork, "");
            } else {
                majorController.loadData(null, pageSize, keywork, department_id);
            }
        });

        $('body').on('click', '#btnSearch', function () {


            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            var department_id = $('#select_department_id').val();
            if (department_id == null) {
                majorController.loadData(null, pageSize, keywork, "");
            } else {
                majorController.loadData(null, pageSize, keywork, department_id);
            }
        })


        $('#select_department_id').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            var department_id = $('#select_department_id').val();
            if (department_id == null) {
                majorController.loadData(null, pageSize, keywork, "");
            } else {
                majorController.loadData(null, pageSize, keywork, department_id);
            }

        })


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
                    majorController.Delete(id);
                }
            });
        });


        $('body').on('click', '.btnDetail', function (e) {
            var id = $(this).data('id');
            majorController.Detail(id);
        });

        pageSize = $('#pageSize').val();
        majorController.loadData(null, pageSize, "", "");
    },

    //page, pageSize, keywork, active
    loadData: function (page, pageSize, keywork, department_id ) {
        $.ajax({
            url: '/Admin/Majors/GetMajorData',
            type: 'GET',
            data: { page: page, pageSize: pageSize, keywork: keywork, department_id: department_id },
            success: function (res) {
                if (res.TotalItem >= 0) {
                    var item = res.Data;
                    var html = "";
                    for (let i = 0; i < res.Data.length; i++) {

                        html += "<tr>";
                        html += ` <td>${i + 1}</td>`;
                        html += ` <td>${item[i].major.code}</td>`;
                        html += ` <td>${item[i].major.name}</td>`;
                        html += ` <td>${item[i].department.name}</td>`;
                        html += `<td>`;
                        html += `<button data-id="${item[i].major.major_id}" class="btn btn-danger btnDelete" title="Xóa" ><i class="fa-solid fa-trash"></i></button> | `
                        html += `<a  href="/Admin/Majors/Edit/${item[i].major.major_id}"  class="btn btn-success btnEdit" title="Cập nhật"><i class="fa-solid fa-pen-to-square"></i></a> | 
                                <button data-id="${item[i].major.major_id}" class="btn btn-info btnDetail" data-toggle="modal" data-target="#exampleModal" title="Xem chi tiết" ><i class="fa-solid fa-eye"></i></button>
                                </td >`
                        html += `</tr>`;
                    }
                }
                $('#show_data').html(html);
                majorController.Pagination(res.CurrentPage, res.NumberPage, res.PageSize);
            }
        })
    },
    Pagination: function (currentPage, numberpage, pageSize) {
        if (numberpage > 0) {
            var str = `<nav aria-label="Page navigation example"><ul class="pagination">`;
            if (currentPage != 1) {
                str += ` <li class="page-item"><a class="page-link" onclick="majorController.NextPage(${currentPage - 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-left"></i></a></li>`;
            }
            for (let i = 1; i <= numberpage; i++) {
                if (currentPage === i) {
                    str += `<li class="page-item active"><a class="page-link" href="javascript:void(0);">${i}</a></li>`;
                } else {

                    str += `<li class="page-item"><a class="page-link" onclick="majorController.NextPage(${i}, ${pageSize})" href="javascript:void(0);">${i}</a></li>`;
                }
            }

            if (currentPage != numberpage) {
                str += ` <li class="page-item"><a class="page-link" onclick="majorController.NextPage(${currentPage + 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-right"></i></a></li>`;
            }
            str += `</ul></nav>`;
            $('#pagination').html(str);
        }
    },

    NextPage: function (page, pageSize) {
        var keywork = $('#keywork').val();
        majorController.loadData(page, pageSize, keywork);
        var department_id = $('#select_department_id').val();
        if (department_id == null) {
            majorController.loadData(page, pageSize, keywork, "");
        } else {
            majorController.loadData(page, pageSize, keywork, department_id);
        }
    },

    Delete: function (id) {
        $.ajax({
            url: "/Admin/Majors/Delete",
            type: "POST",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    pageSize = $('#pageSize').val();
                    majorController.loadData(null, pageSize, "");
                    Swal.fire(
                        'Đã Xóa!',
                        'Bạn đã xóa bộ môn thành công',
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
            url: "/Admin/Majors/Detail",
            type: "GET",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    $('#code').text(res.Data.major.code);
                    $('#name').text(res.Data.major.name);
                    $('#department').text(res.Data.department.name);
                    $('#decs').text(res.Data.major.name);
                } else {
                    alert("Có lỗi");
                }
            }
        });
    },

};

majorController.Init();