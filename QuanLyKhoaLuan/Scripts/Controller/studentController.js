var studentController = {
    init: function () {
        studentController.registerEvent();
    },

    registerEvent: function () {

        $('#pageSize').change(function () {
       
            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var active = $('#active').val();
            var department_id = $('#select_department_id').val();
            var major_id = $('#select_major_id').val();
            var class_id = $('#select_class_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            studentController.loadData(null, pageSize, keywork, active, department_id, major_id, class_id, shool_year_id);


        });

        $('body').on('click', '#btnSearch', function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var active = $('#active').val();
            var department_id = $('#select_department_id').val();
            var major_id = $('#select_major_id').val();
            var class_id = $('#select_class_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            studentController.loadData(null, pageSize, keywork, active, department_id, major_id, class_id, shool_year_id);

        });

        $('#active').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var active = $('#active').val();
            var department_id = $('#select_department_id').val();
            var major_id = $('#select_major_id').val();
            var class_id = $('#select_class_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            studentController.loadData(null, pageSize, keywork, active, department_id, major_id, class_id, shool_year_id);

        })

        $('#select_department_id').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var active = $('#active').val();
            var department_id = $('#select_department_id').val();
            var major_id = $('#select_major_id').val();
            var class_id = $('#select_class_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            studentController.loadData(null, pageSize, keywork, active, department_id, major_id, class_id, shool_year_id);

        });

        $('#select_major_id').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var active = $('#active').val();
            var department_id = $('#select_department_id').val();
            var major_id = $('#select_major_id').val();
            var class_id = $('#select_class_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            studentController.loadData(null, pageSize, keywork, active, department_id, major_id, class_id, shool_year_id);

        });

        $('#select_class_id').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var active = $('#active').val();
            var department_id = $('#select_department_id').val();
            var major_id = $('#select_major_id').val();
            var class_id = $('#select_class_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            studentController.loadData(null, pageSize, keywork, active, department_id, major_id, class_id, shool_year_id);

        });

        $('#select_shoolyear_id').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var active = $('#active').val();
            var department_id = $('#select_department_id').val();
            var major_id = $('#select_major_id').val();
            var class_id = $('#select_class_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            studentController.loadData(null, pageSize, keywork, active, department_id, major_id, class_id, shool_year_id);

        });

        $('body').on('click', '#btnactive', function (e) {


            var id = $(this).data('id');
            studentController.UpdateActive(id);
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
                    studentController.Delete(id)
                }
            });
        });

        $('body').on('click', '.btnDetail', function (e) {
            var id = $(this).data('id');
            studentController.Detail(id);
        });

        pageSize = $('#pageSize').val();
        studentController.loadData(null, pageSize, "", "", "", "", "", "");
    },

    //page, pageSize, keywork, department_id, active
    loadData: function (page, pageSize, keywork, active, department_id, major_id, class_id, shool_year_id ) {
    
        $.ajax({
            url: '/Admin/Students/GetStudentData',
            type: 'GET',

            data: { page: page, pageSize: pageSize, keywork: keywork, active: active, department_id: department_id, major_id: major_id, class_id: class_id, shool_year_id: shool_year_id  },

            success: function (res) {
                if (res.TotalItem >= 0) {

                    var item = res.Data;
                    var html = "";
                    for (let i = 0; i < res.Data.length; i++) {

                        html += "<tr>";
                        html += `<td align="center"><img class="avatar_user_index" src="${item[i].user.avatar}" /></td >`;
                        html += ` <td>${item[i].student.code}</td>`;
                        html += `<td>`;
                        html += `<p>${item[i].student.full_name}</p>`;
                        html += `<p>${item[i].student.email}</p>`;
                        if (item[i].student.phone == null) {
                            html += `<p>-</p>`;
                        } else {
                            html += `<p>${item[i].student.phone}</p>`;
                        }

                        html += ` </td>`;
                        html += `<td>`;
                        html += `<p>${item[i].classes.name}</p>`;
                        html += `<p>${item[i].major.name}</p>`;
                        html += `<p>${item[i].departments.name}</p>`;

                        html += ` </td>`;
                        html += `<td>${item[i].school_year.name}</td>`;
                        if (item[i].user.active) {
                            html += `<td align="center"><button data-id="${item[i].user.user_id}" id="btnactive" class="btn btn-sm btn-primary"><i class="fa-regular fa-lock-open"></i></button></td>`;
                        } else {
                            html += `<td align="center"><button data-id="${item[i].user.user_id}" id="btnactive" class="btn btn-sm btn-danger"><i class="fa-regular fa-lock"></i></button></td>`;
                        }

                        html += `<td>`;
                        html += `<button data-id="${item[i].student.student_id}" class="btn btn-danger btnDelete" title="Xóa" ><i class="fa-solid fa-trash"></i></button> | `
                        html += `<a  href="/Admin/Students/Edit/${item[i].student.student_id}"  class="btn btn-success btnEdit" title="Cập nhật"><i class="fa-solid fa-pen-to-square"></i></a>
                                    | <button data-id="${item[i].student.student_id}" class="btn btn-info btnDetail" data-toggle="modal" data-target="#exampleModal" title="Xem chi tiết" ><i class="fa-solid fa-eye"></i></button>
                                    </td >`
                        html += `</tr>`;
                    }
                }
                $('#show_data').html(html);
                studentController.Pagination(res.CurrentPage, res.NumberPage, res.PageSize);
            }
        })
    },

    Pagination: function (currentPage, numberpage, pageSize) {
        if (numberpage > 0) {
            var str = `<nav aria-label="Page navigation example"><ul class="pagination">`;
            if (currentPage != 1) {
                str += ` <li class="page-item"><a class="page-link" onclick="studentController.NextPage(${currentPage - 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-left"></i></a></li>`;
            }
            for (let i = 1; i <= numberpage; i++) {
                if (currentPage === i) {
                    str += `<li class="page-item active"><a class="page-link" href="javascript:void(0);">${i}</a></li>`;
                } else {

                    str += `<li class="page-item"><a class="page-link" onclick="studentController.NextPage(${i}, ${pageSize})" href="javascript:void(0);">${i}</a></li>`;
                }
            }

            if (currentPage != numberpage) {
                str += ` <li class="page-item"><a class="page-link" onclick="studentController.NextPage(${currentPage + 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-right"></i></a></li>`;
            }
            str += `</ul></nav>`;
            $('#pagination').html(str);
        }
    },

    NextPage: function (page, pageSize) {


        var keywork = $('#keywork').val();
        pageSize = $('#pageSize').val();
        var active = $('#active').val();
        var department_id = $('#select_department_id').val();
        var major_id = $('#select_major_id').val();
        var class_id = $('#select_class_id').val();
        var shool_year_id = $('#select_shoolyear_id').val();

        studentController.loadData(page, pageSize, keywork, active, department_id, major_id, class_id, shool_year_id);

    },

    UpdateActive: function (user_id) {
        $.ajax({
            url: "/Admin/Students/UpdateActive",
            type: "POST",
            data: { id: user_id },
            success: function (res) {
                if (res.Success) {
                    pageSize = $('#pageSize').val();
                    studentController.loadData(null, pageSize, "", "", "", "", "", "");
                } else {
                    console.log("Co Loi");
                }
            }
        });
    },

    Delete: function (id) {
        $.ajax({
            url: "/Admin/Students/Delete",
            type: "POST",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    pageSize = $('#pageSize').val();
                    studentController.loadData(null, pageSize, "", "", "", "", "", "");
                    Swal.fire(
                        'Đã Xóa!',
                        'Sinh viên đã xóa.',
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
            url: "/Admin/Students/Detail",
            type: "GET",
            data: { id: id },
            success: function (res) {
                if (res.Success) {


                    $('#avatarDetail').attr("src", res.Data.user.avatar);
                    var date = studentController.getDateIfDate(res.Data.student.birthday);
                    var gender = "Nam";
                    if (res.Data.student.gender == 0) {
                        gender = "Nữ";
                    }

                    $('#code').text(res.Data.student.code);
                    $('#full_name').text(res.Data.student.full_name);
                    $('#email').text(res.Data.student.email);
                    $('#birthday').text(date);
                    $('#phone').text(res.Data.student.phone);
                    $('#gender').text(gender);
                    $('#address').text(res.Data.student.address);
                    $('#department').text(res.Data.departments.name);
                    $('#major').text(res.Data.major.name);
                    $('#classes').text(res.Data.classes.name);
                    $('#shool_year').text(res.Data.school_year.name);


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

studentController.init();