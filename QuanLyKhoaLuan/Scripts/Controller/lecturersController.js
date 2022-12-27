var lecturersController = {
    init: function () {
        lecturersController.registerEvent();
    },

    registerEvent: function () {

        $('#pageSize').change(function () {
            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            var department_id = $('#select_department_id').val();
            if (department_id == null) {
                lecturersController.loadData(null, pageSize, keywork, "");
            } else {
                lecturersController.loadData(null, pageSize, keywork, department_id);
            }
        });


        $('body').on('click', '#btnSearch', function () {


            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();


            var active = $('#active').val();
            var department_id = $('#select_department_id').val();

            if (department_id == null) {
                if (active == null) {
                    lecturersController.loadData(null, pageSize, keywork, "", "");
                } else {
                    lecturersController.loadData(null, pageSize, keywork, "", active);
                }
            } else {
                if (active == null) {
                    lecturersController.loadData(null, pageSize, keywork, department_id, "");
                } else {
                    lecturersController.loadData(null, pageSize, keywork, department_id, active);
                }
            }

        });

        $('#select_department_id').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            var active = $('#active').val();
            var department_id = $('#select_department_id').val();

            if (department_id == null) {
                if (active == null) {
                    lecturersController.loadData(null, pageSize, keywork, "", "");
                } else {
                    lecturersController.loadData(null, pageSize, keywork, "", active);
                }
            } else {
                if (active == null) {
                    lecturersController.loadData(null, pageSize, keywork, department_id, "");
                } else {
                    lecturersController.loadData(null, pageSize, keywork, department_id, active);
                }
            }

        });

        $('#active').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var active = $('#active').val();
            var department_id = $('#select_department_id').val();

            if (department_id == null) {
                if (active == null) {
                    lecturersController.loadData(null, pageSize, keywork, "", "");
                } else {
                    lecturersController.loadData(null, pageSize, keywork, "" ,active);
                }
            } else {
                if (active == null) {
                    lecturersController.loadData(null, pageSize, keywork, department_id, "");
                } else {
                    lecturersController.loadData(null, pageSize, keywork, department_id, active);
                }
            }
          
        })

        $('body').on('click', '#btnactive', function (e) {


            var id = $(this).data('id');
            lecturersController.UpdateActive(id);
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
                    lecturersController.Delete(id)
                }
            });
        });

        $('body').on('click', '.btnDetail', function (e) {
            var id = $(this).data('id');
            lecturersController.Detail(id);
        });

        pageSize = $('#pageSize').val();
        lecturersController.loadData(null, pageSize, "", "", "");
    },

    //page, pageSize, keywork, active
    loadData: function (page, pageSize, keywork, department_id, active) {
        $.ajax({
            url: '/Admin/Lecturers/GetLecturersData',
            type: 'GET',
            //page: page, pageSize: pageSize, keywork: keywork, active: active
            data: { page: page, pageSize: pageSize, keywork: keywork, department_id: department_id, active: active },
            success: function (res) {
                if (res.TotalItem >= 0) {
                    var item = res.Data;
                    var html = "";
                    for (let i = 0; i < res.Data.length; i++) {

                        html += "<tr>";
                        html += `<td align="center"><img class="avatar_user_index" src="${item[i].user.avatar}" /></td >`;
                        html += ` <td>${item[i].lecture.code}</td>`;
                        html += `<td>`;
                        html += `<p>${item[i].lecture.full_name}</p>`;
                        html += `<p>${item[i].lecture.email}</p>`;
                        if (item[i].lecture.phone == null) {
                            html += `<p>-</p>`;
                        } else {
                            html += `<p>${item[i].lecture.phone}</p>`;
                        }

                        html += ` </td>`;
                        html += `<td>${item[i].department.name}</td>`;
                        if (item[i].user.active) {
                            html += `<td align="center"><button data-id="${item[i].user.user_id}" id="btnactive" class="btn btn-sm btn-primary"><i class="fa-regular fa-lock-open"></i></button></td>`;
                        } else {
                            html += `<td align="center"><button data-id="${item[i].user.user_id}" id="btnactive" class="btn btn-sm btn-danger"><i class="fa-regular fa-lock"></i></button></td>`;
                        }

                        html += `<td>`;
                        html += `<button data-id="${item[i].lecture.lecturer_id}" class="btn btn-danger btnDelete" title="Xóa" ><i class="fa-solid fa-trash"></i></button> | `
                        html += `<a  href="/Admin/Lecturers/Edit/${item[i].lecture.lecturer_id}"  class="btn btn-success btnEdit" title="Cập nhật"><i class="fa-solid fa-pen-to-square"></i></a>
                                    | <button data-id="${item[i].lecture.lecturer_id}" class="btn btn-info btnDetail" data-toggle="modal" data-target="#exampleModal" title="Xem chi tiết" ><i class="fa-solid fa-eye"></i></button>
                                    </td >`
                        html += `</tr>`;
                    }
                }
                $('#show_data').html(html);
                lecturersController.Pagination(res.CurrentPage, res.NumberPage, res.PageSize);
            }
        })
    },
    Pagination: function (currentPage, numberpage, pageSize) {
        if (numberpage > 0) {
            var str = `<nav aria-label="Page navigation example"><ul class="pagination">`;
            if (currentPage != 1) {
                str += ` <li class="page-item"><a class="page-link" onclick="lecturersController.NextPage(${currentPage - 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-left"></i></a></li>`;
            }
            for (let i = 1; i <= numberpage; i++) {
                if (currentPage === i) {
                    str += `<li class="page-item active"><a class="page-link" href="javascript:void(0);">${i}</a></li>`;
                } else {

                    str += `<li class="page-item"><a class="page-link" onclick="lecturersController.NextPage(${i}, ${pageSize})" href="javascript:void(0);">${i}</a></li>`;
                }
            }

            if (currentPage != numberpage) {
                str += ` <li class="page-item"><a class="page-link" onclick="lecturersController.NextPage(${currentPage + 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-right"></i></a></li>`;
            }
            str += `</ul></nav>`;
            $('#pagination').html(str);
        }
    },

    NextPage: function (page, pageSize) {
        var keywork = $('#keywork').val();

        var active = $('#active').val();
        var department_id = $('#select_department_id').val();

        if (department_id == null) {
            if (active == null) {
                lecturersController.loadData(page, pageSize, keywork, "", "");
            } else {
                lecturersController.loadData(page, pageSize, keywork, "", active);
            }
        } else {
            if (active == null) {
                lecturersController.loadData(page, pageSize, keywork, department_id, "");
            } else {
                lecturersController.loadData(page, pageSize, keywork, department_id, active);
            }
        }
    },

    UpdateActive: function (user_id) {
        $.ajax({
            url: "/Admin/Lecturers/UpdateActive",
            type: "POST",
            data: { id: user_id },
            success: function (res) {
                if (res.Success) {
                    pageSize = $('#pageSize').val();
                    lecturersController.loadData(null, pageSize, "", "");
                } else {
                    console.log("Co Loi");
                }
            }
        });
    },

    Delete: function (id) {
        $.ajax({
            url: "/Admin/Lecturers/Delete",
            type: "POST",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    pageSize = $('#pageSize').val();
                    lecturersController.loadData(null, pageSize, "", "");
                    Swal.fire(
                        'Đã Xóa!',
                        'Giảng viên đã xóa.',
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
            url: "/Admin/Lecturers/Detail",
            type: "GET",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    var date = lecturersController.getDateIfDate(res.Data.lecture.birthday);
                    var gender = "Nam";
                    if (res.Data.lecture.gender == 0) {
                        gender = "Nữ";
                    }
                    $('#avatarDetail').attr("src", res.Data.user.avatar);
                    $('#code').text(res.Data.lecture.code);
                    $('#full_name').text(res.Data.lecture.full_name);
                    $('#email').text(res.Data.lecture.email);
                    $('#birthday').text(date);
                    $('#phone').text(res.Data.lecture.phone);
                    $('#gender').text(gender);
                    $('#address').text(res.Data.lecture.address);
                    $('#department').text(res.Data.department.name);
                } else {
                    alert("Có lỗi");
                }
            }
        });
    },

    getDateIfDate: function (d) {
        var m = d.match(/\/Date\((\d+)\)\//);
        return m ? (new Date(+m[1])).toLocaleDateString('es-SV', { month: '2-digit', day: '2-digit', year: 'numeric' }) : d;
    },
}

lecturersController.init();