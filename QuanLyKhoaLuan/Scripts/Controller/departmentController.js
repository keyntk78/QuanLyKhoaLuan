var departmentController = {
    init: function () {

        departmentController.registerEvent();
    },
    registerEvent: function () {

        $('#pageSize').change(function () {
            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();

            departmentController.loadData(null, pageSize, keywork);
        });

        $('body').on('click', '#btnSearch', function () {


            var keywork = $('#keywork').val();

            departmentController.loadData(null, pageSize, keywork);
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
                    departmentController.Delete(id)
                }
            });
        });



        $('body').on('click', '.btnDetail', function (e) {
            var id = $(this).data('id');
            departmentController.Detail(id);          
        });


        pageSize = $('#pageSize').val();
        departmentController.loadData(null, pageSize, "");
    },

    loadData: function (page, pageSize, keywork) {
        $.ajax({
            url: '/Admin/Departments/GetDepartmentsData',
            type: 'GET',
            data: { page: page, pageSize: pageSize, keywork: keywork },
            success: function (res) {
                if (res.TotalItem >= 0) {
                    var item = res.Data;
                    var html = "";
                    for (let i = 0; i < res.Data.length; i++) {
                        var date = departmentController.getDateIfDate(item[i].founding_date);

                        html += "<tr>";
                        html += ` <td>${i + 1}</td>`;
                        html += ` <td>${item[i].code}</td>`;
                        html += ` <td>${item[i].name}</td>`;
                        html += ` <td>${date}</td>`;
                        html += `<td>`;
                        html += `<button data-id="${item[i].department_id}" class="btn btn-danger btnDelete" title="Xóa" ><i class="fa-solid fa-trash"></i></button> | `
                        html += `<a  href="/Admin/Departments/Edit/${item[i].department_id}"  class="btn btn-success btnEdit" title="Cập nhật"><i class="fa-solid fa-pen-to-square"></i></a> | 
                                <button data-id="${item[i].department_id}" class="btn btn-info btnDetail" data-toggle="modal" data-target="#exampleModal" title="Xem chi tiết" ><i class="fa-solid fa-eye"></i></button>
                                </td >`
                        html += `</tr>`;
                    }
                }
                $('#show_data').html(html);
                departmentController.Pagination(res.CurrentPage, res.NumberPage, res.PageSize);
            }
        })
    },

    isoDateReviver: function (value) {
        if (typeof value === 'string') {
            var a = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)(?:([\+-])(\d{2})\:(\d{2}))?Z?$/.exec(value);
            if (a) {
                var utcMilliseconds = Date.UTC(+a[1], +a[2] - 1, +a[3], +a[4], +a[5], +a[6]);
                return new Date(utcMilliseconds);
            }
        }
        return value;
    },

    getDateIfDate: function (d) {
        var m = d.match(/\/Date\((\d+)\)\//);
        return m ? (new Date(+m[1])).toLocaleDateString('es-SV', { month: '2-digit', day: '2-digit', year: 'numeric' }) : d;
    },

    Pagination: function (currentPage, numberpage, pageSize) {
        if (numberpage > 0) {
            var str = `<nav aria-label="Page navigation example"><ul class="pagination">`;
            if (currentPage != 1) {
                str += ` <li class="page-item"><a class="page-link" onclick="departmentController.NextPage(${currentPage - 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-left"></i></a></li>`;
            }
            for (let i = 1; i <= numberpage; i++) {
                if (currentPage === i) {
                    str += `<li class="page-item active"><a class="page-link" href="javascript:void(0);">${i}</a></li>`;
                } else {

                    str += `<li class="page-item"><a class="page-link" onclick="departmentController.NextPage(${i}, ${pageSize})" href="javascript:void(0);">${i}</a></li>`;
                }
            }

            if (currentPage != numberpage) {
                str += ` <li class="page-item"><a class="page-link" onclick="departmentController.NextPage(${currentPage + 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-right"></i></a></li>`;
            }
            str += `</ul></nav>`;
            $('#pagination').html(str);
        }
    },

    NextPage: function (page, pageSize) {
        var keywork = $('#keywork').val();
        console.log(page);
        departmentController.loadData(page, pageSize);

    },

    Delete: function (id) {
        $.ajax({
            url: "/Admin/Departments/Delete",
            type: "POST",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    pageSize = $('#pageSize').val();
                    departmentController.loadData(null, pageSize, "");
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

    Detail: function (id) {
        $.ajax({
            url: "/Admin/Departments/Detail",
            type: "GET",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    $('#code').text(res.Data.code);
                    $('#name').text(res.Data.name);
                    $('#f-date').text(departmentController.getDateIfDate(res.Data.founding_date));
                    $('#decs').text(res.Data.description);
                } else {
                    alert("Có lỗi");
                }
            }
        });
    },

};

departmentController.init();