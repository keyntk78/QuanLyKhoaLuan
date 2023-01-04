var instructionalThesisController = {
    init: function () {

        instructionalThesisController.registerEvent();
    },
    registerEvent: function () {
        $('#pageSize').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var status = $('#status').val();
            var major_id = $('#select_major_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            instructionalThesisController.loadData(null, pageSize, keywork, status, major_id, shool_year_id);


        });

        $('body').on('click', '#btnSearch', function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var status = $('#status').val();
            var major_id = $('#select_major_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            instructionalThesisController.loadData(null, pageSize, keywork, status, major_id, shool_year_id);

        });

        $('#status').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var status = $('#status').val();
            var major_id = $('#select_major_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            instructionalThesisController.loadData(null, pageSize, keywork, status, major_id, shool_year_id);

        })


        $('#select_major_id').change(function () {

            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var status = $('#status').val();
            var major_id = $('#select_major_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            instructionalThesisController.loadData(null, pageSize, keywork, status, major_id, shool_year_id);

        });

        $('#select_shoolyear_id').change(function () {


            var keywork = $('#keywork').val();
            pageSize = $('#pageSize').val();
            var status = $('#status').val();
            var major_id = $('#select_major_id').val();
            var shool_year_id = $('#select_shoolyear_id').val();

            instructionalThesisController.loadData(null, pageSize, keywork, status, major_id, shool_year_id);

        });

        var pageSize = $('#pageSize').val();
        instructionalThesisController.loadData(null, pageSize, "", "", "", "");
    },

    //page, pageSize, keywork, status, major_id, shool_year_id
    loadData: function (page, pageSize, keywork, status, major_id, shool_year_id) {

        $.ajax({
            url: '/Lecture/InstructionalThesis/GetThesesData',
            type: 'GET',

            //data: { page: page, pageSize: pageSize, keywork: keywork, status: status, major_id: major_id, shool_year_id: shool_year_id },
            data: { page: page, pageSize: pageSize, keywork: keywork, status: status, major_id: major_id, shool_year_id: shool_year_id },
            success: function (res) {
                if (res.TotalItem >= 0) {

                    var item = res.Data;
                    var html = "";
                    console.log(item);
                    for (let i = 0; i < res.Data.length; i++) {
                        var status = `<span class="badge badge-success">Hoàn thành</i></span>`;
                        if (item[i].theses.status == false) {
                            status = `<span class="badge badge-danger">Chưa hoàn thành</i></span>`;
                        }

                        var total_score = `<span class="badge badge-danger">Chưa có</span>`;
                        if (item[i].theses.total_score != null) {
                            total_score = `<span class="badge badge-success">${item[i].theses.total_score}</span>`;
                        }

                        var result = `<span class="badge badge-danger">Chưa có</span>`;
                        if (item[i].theses.result != null) {
                            if (item[i].theses.result == 1) {
                                result = `<span class="badge badge-success">Đạt</span>`;
                            } else {
                                result = `<span class="badge badge-success">Chưa đạt</span>`;
                            }
                        }


                        html += "<tr>";
                        html += ` <td>${item[i].theses.code}</td>`;
                        html += ` <td>${item[i].topic.name}</td>`;
                        html += ` <td>${item[i].major.name}</td>`;
                        html += ` <td>${item[i].school_year.name}</td>`;
                        html += ` <td>${instructionalThesisController.getDateIfDate(item[i].theses.start_date)} đến ${instructionalThesisController.getDateIfDate(item[i].theses.end_date)} </td>`;
                        html += ` <td>${item[i].student.full_name}</td>`;
                        html += ` <td>${status}</td>`;
                        html += ` <td>${total_score}</td>`;
                        html += ` <td>${result}</td>`;

                        html += `<td>`;
                        html += `<a  href="/Lecture/InstructionalThesis/Detail/${item[i].theses.thesis_id}"  class="btn btn-info btnEdit" title="Xem chi tiết"><i class="fa-solid fa-eye"></i></a>
                                    </td >`
                        html += `</tr>`;
                    }
                }
                $('#show_data').html(html);
                instructionalThesisController.Pagination(res.CurrentPage, res.NumberPage, res.PageSize);
            },

        })
    },
    Pagination: function (currentPage, numberpage, pageSize) {
        if (numberpage > 0) {
            var str = `<nav aria-label="Page navigation example"><ul class="pagination">`;
            if (currentPage != 1) {
                str += ` <li class="page-item"><a class="page-link" onclick="instructionalThesisController.NextPage(${currentPage - 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-left"></i></a></li>`;
            }
            for (let i = 1; i <= numberpage; i++) {
                if (currentPage === i) {
                    str += `<li class="page-item active"><a class="page-link" href="javascript:void(0);">${i}</a></li>`;
                } else {

                    str += `<li class="page-item"><a class="page-link" onclick="instructionalThesisController.NextPage(${i}, ${pageSize})" href="javascript:void(0);">${i}</a></li>`;
                }
            }

            if (currentPage != numberpage) {
                str += ` <li class="page-item"><a class="page-link" onclick="instructionalThesisController.NextPage(${currentPage + 1}, ${pageSize})" href="javascript:void(0);"><i class="fa-solid fa-chevron-right"></i></a></li>`;
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

        instructionalThesisController.loadData(page, pageSize, keywork, status, major_id, shool_year_id);

    },
    getDateIfDate: function (d) {
        var m = d.match(/\/Date\((\d+)\)\//);
        return m ? (new Date(+m[1])).toLocaleDateString('es-SV', { month: '2-digit', day: '2-digit', year: 'numeric' }) : d;
    },

}

instructionalThesisController.init();