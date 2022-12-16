var userController = {
    init: function () {
        userController.registerEvent();
    },
    registerEvent: function () {

        $('#pageSize').change(function () {
            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            var active = $('#active').val();
            if (active == null) {
                userController.loadData(null, pageSize, keywork, "");
            } else {
                userController.loadData(null, pageSize, keywork, active);
            }

        })

        $('body').on('click', '#btnSearch', function () {


            var keywork = $('#keywork').val();

            var active = $('#active').val();
            if (active == null) {
                userController.loadData(null, pageSize, keywork, "");
            } else {
                userController.loadData(null, pageSize, keywork, active);
            }

        })

        $('#active').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var active = $('#active').val();



            if (active == null) {
                userController.loadData(null, pageSize, keywork, "");
            } else {
                userController.loadData(null, pageSize, keywork, active);
            }

        })

        $('body').on('click', '#btnactive', function (e) {


            var id = $(this).data('id');
            userController.UpdateActive(id);
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
                    userController.Delete(id)
                }
            });
        });


        pageSize = $('#pageSize').val();
        userController.loadData(null, pageSize, "", "");
    },
    /*    loadData: function (page, pageSize, keywork, active) {*/
    loadData: function (page, pageSize, keywork, active) {
        $.ajax({
            url: '/Admin/Users/GetUsersData',
            type: 'GET',
            data: { page: page, pageSize: pageSize, keywork: keywork, active: active },
            success: function (res) {
                if (res.TotalItem >= 0) {
                    var item = res.Data;
                    var html = "";
                    for (let i = 0; i < res.Data.length; i++) {

                        html += "<tr>";
                        html += `<td align="center"><img class="avatar_user_index" src="${item[i].user.avatar}" /></td >`;
                        html += ` <td>${item[i].user.username}</td>`;
                        html += `<td>`;
                        html += `<p>${item[i].user.full_name}</p>`;
                        html += `<p>${item[i].user.email}</p>`;
                        if (item[i].user.phone == null) {
                            html += `<p>-</p>`;
                        } else {
                            html += `<p>${item[i].user.phone}</p>`;
                        }

                        html += ` </td>`;
                        if (item[i].user.active) {
                            html += `<td align="center"><button data-id="${item[i].user.user_id}" id="btnactive" class="btn btn-sm btn-primary"><i class="fa-regular fa-lock-open"></i></button></td>`;
                        } else {
                            html += `<td align="center"><button data-id="${item[i].user.user_id}" id="btnactive" class="btn btn-sm btn-danger"><i class="fa-regular fa-lock"></i></button></td>`;
                        }
                        html += `<td>${item[i].role_name}</td>`;
                        html += `<td>`;
                        html += `<button data-id="${item[i].user.user_id}" class="btn btn-danger btnDelete" title="Xóa" ><i class="fa-solid fa-trash"></i></button> | `
                        html += `<a  href="/Admin/Users/Edit/${item[i].user.user_id}"  class="btn btn-success btnEdit" title="Cập nhật"><i class="fa-solid fa-pen-to-square"></i></a></td >`
                        html += `</tr>`;
                    }
                }
                $('#show_data').html(html);
                userController.Pagination(res.CurrentPage, res.NumberPage, res.PageSize);
            }
        })
    },

    Pagination: function (currentPage, numberpage, pageSize) {
        if (numberpage > 0) {
            var str = `<nav aria-label="Page navigation example"><ul class="pagination">`;
            if (currentPage != 1) {
                str += ` <li class="page-item"><a class="page-link" onclick="userController.NextPage(${currentPage - 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-left"></i></a></li>`;
            }
            for (let i = 1; i <= numberpage; i++) {
                if (currentPage === i) {
                    str += `<li class="page-item active"><a class="page-link" href="javascript:void(0);">${i}</a></li>`;
                } else {

                    str += `<li class="page-item"><a class="page-link" onclick="userController.NextPage(${i}, ${pageSize})" href="javascript:void(0);">${i}</a></li>`;
                }
            }

            if (currentPage != numberpage) {
                str += ` <li class="page-item"><a class="page-link" onclick="userController.NextPage(${currentPage + 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-right"></i></a></li>`;
            }
            str += `</ul></nav>`;
            $('#pagination').html(str);
        }
    },

    NextPage: function (page, pageSize) {
        var keywork = $('#keywork').val();


        var active = $('#active').val();

        if (active == null) {
            userController.loadData(page, pageSize, keywork, "");
        } else {
            userController.loadData(page, pageSize, keywork, active);
        }

    },

    Delete: function (user_id) {
        $.ajax({
            url: "/Admin/Users/Delete",
            type: "POST",
            data: { user_id: user_id },
            success: function (res) {
                if (res.Success) {
                    pageSize = $('#pageSize').val();
                    userController.loadData(null, pageSize, "", "");
                    Swal.fire(
                        'Đã Xóa!',
                        'Quản trị viên đã xóa.',
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

    UpdateActive: function (user_id) {
        $.ajax({
            url: "/Admin/Users/UpdateActive",
            type: "POST",
            data: { id: user_id },
            success: function (res) {
                if (res.Success) {
                    pageSize = $('#pageSize').val();
                    userController.loadData(null, pageSize, "", "");
                } else {
                    console.log("Co Loi");
                }
            }
        });
    }

}
userController.init();