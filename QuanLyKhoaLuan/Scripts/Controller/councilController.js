var councilController = {
    init: function () {
        councilController.registerEvent();
    },

    registerEvent: function () {


        $('#pageSize').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            councilController.loadData(null, pageSize, keywork, shool_year_id);
        });

        $('body').on('click', '#btnSearch', function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            councilController.loadData(null, pageSize, keywork, shool_year_id);

        });

        $('#select_shoolyear_id').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            councilController.loadData(null, pageSize, keywork, shool_year_id);
        });

        $('body').on('click', '.btnDetail', function (e) {
            var id = $(this).data('id');
            councilController.Detail(id);
        });

        $('body').on('click', '#btnactive', function (e) {
            var id = $(this).data('id');
            councilController.UpdateActive(id);
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
                alert(id);
                if (result.value) {
                    councilController.Delete(id)
                }
            });
        });

        pageSize = $('#pageSize').val();
        councilController.loadData(null, pageSize, "","");
    },
    //page, pageSize, keywork, department_id
    loadData: function (page, pageSize, keywork, shool_year_id) {
        $.ajax({
            url: '/Admin/Councils/GetCouncilData',
            type: 'GET',
            data: { page: page, pageSize: pageSize, keywork: keywork, shool_year_id: shool_year_id },
 
            success: function (res) {
                if (res.TotalItem >= 0) {
                  
                    var item = res.Data;
                    var html = "";
                    for (let i = 0; i < res.Data.length; i++) {
                        html += "<tr>";
                        html += ` <td>${i + 1}</td>`;
                        html += ` <td>${item[i].council.council.code}</td>`;
                        html += ` <td>${item[i].council.council.name}</td>`;
                        html += ` <td>${item[i].council.shool_year.name}</td>`;
                        if (item[i].council.council.is_block) {
                            html += `<td align="center"><button data-id="${item[i].council.council.council_id}" id="btnactive" class="btn btn-sm btn-primary"><i class="fa-regular fa-lock-open"></i></button></td>`;
                        } else {
                            html += `<td align="center"><button data-id="${item[i].council.council.council_id}" id="btnactive" class="btn btn-sm btn-danger"><i class="fa-regular fa-lock"></i></button></td>`;
                        }
                        html += `<td>`;
                        for (let j = 0; j < item[i].detail_Councils.length; j++) {
                            html += `<p>${item[i].detail_Councils[j].lecturer.full_name}</p>`;
                        }
                        html += ` </td>`;
                        html += `<td>`;
                        html += `<button data-id="${item[i].council.council.council_id}" class="btn btn-danger btnDelete" title="Xóa" ><i class="fa-solid fa-trash"></i></button> | `
                        html += `<a  href="/Admin/Councils/Edit/${item[i].council.council.council_id}"  class="btn btn-success btnEdit" title="Cập nhật"><i class="fa-solid fa-pen-to-square"></i></a> | 
                                <button data-id="${item[i].council.council.council_id}" class="btn btn-info btnDetail" data-toggle="modal" data-target="#exampleModal" title="Xem chi tiết" ><i class="fa-solid fa-eye"></i></button>
                                </td >`
                        html += `</tr>`;
                    }
              }
                $('#show_data').html(html);
                councilController.Pagination(res.CurrentPage, res.NumberPage, res.PageSize);
            }
        })
    },

    Pagination: function (currentPage, numberpage, pageSize) {
        if (numberpage > 0) {
            var str = `<nav aria-label="Page navigation example"><ul class="pagination">`;
            if (currentPage != 1) {
                str += ` <li class="page-item"><a class="page-link" onclick="councilController.NextPage(${currentPage - 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-left"></i></a></li>`;
            }
            for (let i = 1; i <= numberpage; i++) {
                if (currentPage === i) {
                    str += `<li class="page-item active"><a class="page-link" href="javascript:void(0);">${i}</a></li>`;
                } else {

                    str += `<li class="page-item"><a class="page-link" onclick="councilController.NextPage(${i}, ${pageSize})" href="javascript:void(0);">${i}</a></li>`;
                }
            }

            if (currentPage != numberpage) {
                str += ` <li class="page-item"><a class="page-link" onclick="councilController.NextPage(${currentPage + 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-right"></i></a></li>`;
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

        councilController.loadData(page, pageSize, keywork, shool_year_id);

    },


    Detail: function (id) {
        $.ajax({
            url: "/Admin/Councils/Detail",
            type: "GET",
            data: { id: id },
            success: function (res) {
                if (res.Success) {

                    var status = "Chưa duyệt";
                    if (res.Data.council.council.is_block == true) {
                        status = "Đã chuyệt";
                    }

                    $('#code').text(res.Data.council.council.code);
                    $('#name').text(res.Data.council.council.name);
                    $('#shool_year').text(res.Data.council.shool_year.name);
                    var html = "";
                    for (let i = 0; i < res.Data.detail_Councils.length; i++) {
                        html += ` <p>${res.Data.detail_Councils[i].lecturer.full_name}</p>`;
                        
                    }
                    $('#thanhvien').html(html);
                    $('#decs').text(res.Data.council.council.description);
                    $('#is_block').text(status);


                 
                } else {
                    alert("Có lỗi");
                }
            }
        });
    },

    UpdateActive: function (id) {
        $.ajax({
            url: "/Admin/Councils/UpdateActive",
            type: "POST",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    pageSize = $('#pageSize').val();
                    councilController.loadData(null, pageSize, "", "");
                } else {
                    console.log("Co Loi");
                }
            }
        });
    },

    Delete: function (id) {
        $.ajax({
            url: "/Admin/Councils/Delete",
            type: "POST",
            data: { id: id },
            success: function (res) {
                if (res.Success) {
                    pageSize = $('#pageSize').val();
                    councilController.loadData(null, pageSize, "", "");
                    Swal.fire(
                        'Đã Xóa!',
                        'Hội đồng đã xóa.',
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

};

councilController.init();

